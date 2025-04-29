using Api.Infrastructure.Enums;
using Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Infrastructure.Fixtures;

public static class InitialDataSeeder
{
    /// <summary>
    ///     Initializes the database with seed data
    /// </summary>
    /// <param name="serviceProvider">The application's service provider</param>
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = serviceProvider.GetRequiredService<ENEADbContext>();

        // Create the database if it doesn't exist
        context.Database.EnsureCreated();

        // Seed data only if the database is empty
        if (!context.ChargerGroups.Any())
        {
            SeedUsers(context);
            SeedChargingStations(context);
            SeedChargers(context);
            SeedChargerEvents(context);
        }

    }

    /// <summary>
    ///     Seeds users with mock data
    /// </summary>
    private static void SeedUsers(ENEADbContext context)
    {
        var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Name = "Admin User",
                Email = "admin@example.com",
                Password = HashPassword("Admin123!"),
                Role = UserRole.Admin
            },
            new User { Id = Guid.NewGuid(), Name = "Jan Novák", Email = "jan.novak@example.com", Password = HashPassword("Technician123!") },
            new User { Id = Guid.NewGuid(), Name = "Petr Svoboda", Email = "petr.svoboda@example.com", Password = HashPassword("Technician123!") },
            new User { Id = Guid.NewGuid(), Name = "Jiří Dvořák", Email = "jiri.dvorak@example.com", Password = HashPassword("Customer123!") },
            new User { Id = Guid.NewGuid(), Name = "Martin Černý", Email = "martin.cerny@example.com", Password = HashPassword("Customer123!") },
            new User { Id = Guid.NewGuid(), Name = "Tomáš Procházka", Email = "tomas.prochazka@example.com", Password = HashPassword("Customer123!") }
        };

        // Generate 20 additional random customers
        var random = new Random();
        for (var i = 0; i < 20; i++)
        {
            var firstName = GetRandomFirstName();
            var lastName = GetRandomLastName();
            users.Add(new User { Id = Guid.NewGuid(), Name = $"{firstName} {lastName}", Email = $"{firstName.ToLower()}.{lastName.ToLower()}@example.com" });
        }

        // check for duplicates email
        var emails = new HashSet<string>();
        foreach (var user in users.Where(user => !emails.Add(user.Email)))
        {
            user.Email = $"{user.Name.ToLower().Replace(" ", ".")}{random.Next(1, 100)}@example.com";
        }


        context.Users.AddRange(users);
        context.SaveChanges();
    }

    /// <summary>
    ///     Seeds charging stations with mock data
    /// </summary>
    private static void SeedChargingStations(ENEADbContext context)
    {
        var stations = new List<ChargerGroup>
        {
            new ChargerGroup
            {
                Id = Guid.NewGuid(),
                Name = "City Center Station",
                Latitude = 50.073658,
                Longitude = 14.418540,
                Address = "Václavské náměstí 846/1, 110 00 Praha 1"
            },
            new ChargerGroup
            {
                Id = Guid.NewGuid(),
                Name = "Shopping Mall Station",
                Latitude = 50.103969,
                Longitude = 14.393098,
                Address = "Evropská 2758, 160 00 Praha 6"
            },
            new ChargerGroup
            {
                Id = Guid.NewGuid(),
                Name = "Highway Rest Stop",
                Latitude = 49.994772,
                Longitude = 14.651203,
                Address = "Dálnice D1, Exit 15, 251 01 Říčany"
            },
            new ChargerGroup
            {
                Id = Guid.NewGuid(),
                Name = "Business Park Station",
                Latitude = 50.050278,
                Longitude = 14.460139,
                Address = "Rohanské nábřeží 678/23, 186 00 Praha 8"
            },
            new ChargerGroup
            {
                Id = Guid.NewGuid(),
                Name = "Residential Area Station",
                Latitude = 50.128611,
                Longitude = 14.503333,
                Address = "Lovosická 778/2, 190 00 Praha 9"
            }
        };

        context.ChargerGroups.AddRange(stations);
        context.SaveChanges();
    }

    /// <summary>
    ///     Seeds chargers with mock data
    /// </summary>
    private static void SeedChargers(ENEADbContext context)
    {
        var stations = context.ChargerGroups.ToList();
        var chargers = new List<Charger>();

        // Generate 2-4 chargers for each station
        foreach (var station in stations)
        {
            var chargerCount = new Random().Next(2, 5); // 2-4 chargers per station

            for (var i = 1; i <= chargerCount; i++)
            {
                chargers.Add(new Charger { Id = Guid.NewGuid(), ChargerCode = $"{station.Name.Substring(0, 3).ToUpper()}-{i:D2}", ChargerGroupId = station.Id, CurrentStatus = GetRandomStatus() });
            }
        }

        context.Chargers.AddRange(chargers);
        context.SaveChanges();
    }

    /// <summary>
    ///     Seeds charger events (status changes and charging sessions) with mock data,
    ///     simulating realistic sequences over the past year, based on charger status transitions.
    /// </summary>
    private static void SeedChargerEvents(ENEADbContext context)
    {
        var chargers = context.Chargers.Include(c => c.ChargerGroup).ToList(); // Include group for potential context
        var customers = context.Users.Take(10).ToList();
        var technicians = context.Users.Skip(10).ToList();
        var events = new List<ChargerEvent>();
        var random = new Random();

        // Constants for realistic calculations (adjust as needed)
        const double minEnergyRatePerHour = 7.0; // Min kWh per hour
        const double maxEnergyRatePerHour = 22.0; // Max kWh per hour (typical AC charger range)
        const double pricePerKWh = 0.20; // Price per kWh (consistent with ChargerService)
        var simulationStartDate = DateTime.UtcNow.AddYears(-1).AddDays(-random.Next(1, 30)); // Start simulation ~1 year ago

        if (!customers.Any())
        {
            Console.WriteLine("Warning: No customer users found for seeding charging sessions.");

            // Optionally handle this case, e.g., skip session generation or use other users
        }
        if (!technicians.Any())
        {
            Console.WriteLine("Warning: No technician or admin users found for seeding maintenance events.");

            // Optionally handle this case, e.g., skip maintenance simulation
        }

        foreach (var charger in chargers)
        {
            var currentTime = simulationStartDate;

            // Start with a plausible initial status, maybe Available or OutOfOrder briefly
            var currentStatus = random.NextDouble() < 0.8 ? ChargerStatus.Available : ChargerStatus.OutOfOrder;
            charger.CurrentStatus = currentStatus; // Set initial status for the simulation start

            ChargerEvent? activeSessionStartEvent = null; // Track the start event of an ongoing session

            // Simulate events until the present time
            while (currentTime < DateTime.UtcNow)
            {
                // Advance time randomly: 15 minutes to 2 days (more variability)
                var timeToAdd = TimeSpan.FromMinutes(random.Next(15, 2 * 24 * 60));
                var nextEventTime = currentTime.Add(timeToAdd);

                // Ensure simulation doesn't go beyond the present
                if (nextEventTime >= DateTime.UtcNow)
                {
                    nextEventTime = DateTime.UtcNow;
                }

                // Decide next event based on current status
                switch (currentStatus)
                {
                    case ChargerStatus.Available:
                        // Higher chance to start charging if customers exist, small chance for maintenance
                        var availableAction = random.NextDouble();
                        if (availableAction < 0.6 && customers.Any()) // Start Charging
                        {
                            var user = customers[random.Next(customers.Count)];
                            var oldStatus = currentStatus;
                            currentStatus = ChargerStatus.Charging;
                            currentTime = nextEventTime; // Event happens at this time

                            activeSessionStartEvent = new ChargerEvent
                            {
                                Id = Guid.NewGuid(),
                                ChargerId = charger.Id,
                                EventType = EventType.ChargingSession, // Log session start
                                StartTime = currentTime,
                                EndTime = null, // Not ended yet
                                SessionStatus = ChargingSessionStatus.InProgress,
                                OldStatus = oldStatus, // Status before this event
                                NewStatus = currentStatus, // Status after this event
                                UserId = user.Id,
                                IsCompleted = false,
                                Notes = "Charging session initiated."
                            };
                            events.Add(activeSessionStartEvent);
                        }
                        else if (availableAction < 0.65 && technicians.Any()) // Go to Maintenance (lower chance)
                        {
                            var user = technicians[random.Next(technicians.Count)];
                            var oldStatus = currentStatus;
                            currentStatus = ChargerStatus.OutOfOrder;
                            currentTime = nextEventTime; // Event happens at this time

                            events.Add(new ChargerEvent
                            {
                                Id = Guid.NewGuid(),
                                ChargerId = charger.Id,
                                EventType = EventType.StatusChange,
                                StartTime = currentTime,
                                EndTime = currentTime, // Status change is instantaneous
                                OldStatus = oldStatus,
                                NewStatus = currentStatus,
                                UserId = user.Id,
                                Notes = GetRandomStatusNote(oldStatus, currentStatus) // Generate maintenance reason
                            });
                        }
                        else // Stays Available
                        {
                            currentTime = nextEventTime; // Advance time, no event logged
                        }
                        break;

                    case ChargerStatus.Charging:
                        // Must stop charging eventually
                        if (activeSessionStartEvent != null)
                        {
                            var oldStatus = currentStatus;
                            currentStatus = ChargerStatus.Available; // Becomes available after charging
                            currentTime = nextEventTime; // Event happens at this time

                            // Calculate duration, energy, cost
                            var endTime = currentTime;
                            var duration = endTime - activeSessionStartEvent.StartTime;

                            // Ensure minimum duration (e.g., 5 mins) to avoid tiny/negative values if time jumps are small
                            if (duration < TimeSpan.FromMinutes(5))
                            {
                                duration = TimeSpan.FromMinutes(random.Next(5, 15));
                            }
                            var durationHours = duration.TotalHours;

                            var energyRate = minEnergyRatePerHour + random.NextDouble() * (maxEnergyRatePerHour - minEnergyRatePerHour);
                            var energyConsumed = Math.Max(0, Math.Round(durationHours * energyRate, 2)); // Ensure non-negative
                            var totalPrice = Math.Max(0, Math.Round(energyConsumed * pricePerKWh, 2)); // Ensure non-negative

                            // Update the original session start event
                            activeSessionStartEvent.EndTime = endTime;
                            activeSessionStartEvent.EnergyConsumed = energyConsumed;
                            activeSessionStartEvent.TotalPrice = totalPrice;
                            activeSessionStartEvent.SessionStatus = ChargingSessionStatus.Completed;
                            activeSessionStartEvent.IsCompleted = true;
                            activeSessionStartEvent.Notes = $"Session completed. Charged {energyConsumed:F2} kWh. Cost: {totalPrice:C2}";

                            // We are updating the existing event object in the 'events' list

                            // ChargerService logs a separate event for stop; mimic that if desired
                            // events.Add(new ChargerEvent { ... EventType = EventType.StatusChange, OldStatus = oldStatus, NewStatus = currentStatus ... });

                            activeSessionStartEvent = null; // Clear active session
                        }
                        else
                        {
                            // Defensive: If somehow in Charging state without an active session, force to Available
                            currentStatus = ChargerStatus.Available;
                            currentTime = nextEventTime; // Advance time
                        }
                        break;

                    case ChargerStatus.OutOfOrder:
                        // Must become Available after some time (maintenance duration)
                        // Higher chance to become available if technicians exist
                        if (random.NextDouble() < 0.9 && technicians.Any())
                        {
                            var user = technicians[random.Next(technicians.Count)];
                            var oldStatus = currentStatus;
                            currentStatus = ChargerStatus.Available;
                            currentTime = nextEventTime; // Event happens at this time

                            events.Add(new ChargerEvent
                            {
                                Id = Guid.NewGuid(),
                                ChargerId = charger.Id,
                                EventType = EventType.StatusChange,
                                StartTime = currentTime,
                                EndTime = currentTime,
                                OldStatus = oldStatus,
                                NewStatus = currentStatus,
                                UserId = user.Id,
                                Notes = GetRandomStatusNote(oldStatus, currentStatus) // Generate maintenance completion note
                            });
                        }
                        else // Stays OutOfOrder
                        {
                            currentTime = nextEventTime; // Advance time, no event logged
                        }
                        break;
                }

                // Break if we have simulated past the current time
                if (currentTime >= DateTime.UtcNow)
                {
                    break;
                }
            }

            // Update the charger's final status based on the end of the simulation
            charger.CurrentStatus = currentStatus;

            // If a session was still active at the end of the simulation, mark it as InProgress
            // and ensure the charger's final status reflects this.
            if (activeSessionStartEvent != null && activeSessionStartEvent.EndTime == null)
            {
                charger.CurrentStatus = ChargerStatus.Charging;

                // Optionally add a note indicating it was ongoing at simulation end
                activeSessionStartEvent.Notes += " (Session ongoing at data generation time)";
            }
        }

        context.ChargerEvents.AddRange(events);

        // Save events first
        context.SaveChanges();

        // Update charger entities with their final simulated status
        // EF Core tracks changes to the 'chargers' list objects
        context.SaveChanges();
    }

    #region Helper Methods

    /// <summary>
    ///     Returns a hashed password
    /// </summary>
    private static string HashPassword(string password)
    {
        return password;
    }

    /// <summary>
    ///     Returns a random charger status
    /// </summary>
    private static ChargerStatus GetRandomStatus()
    {
        var statuses = Enum.GetValues(typeof(ChargerStatus));
        return (ChargerStatus)statuses.GetValue(new Random().Next(statuses.Length));
    }

    /// <summary>
    ///     Returns a random status different from the provided one
    /// </summary>
    private static ChargerStatus GetDifferentStatus(ChargerStatus currentStatus)
    {
        var statuses = Enum.GetValues(typeof(ChargerStatus))
            .Cast<ChargerStatus>()
            .Where(s => s != currentStatus)
            .ToArray();

        return statuses[new Random().Next(statuses.Length)];
    }

    /// <summary>
    ///     Returns a random charging session status
    /// </summary>
    private static ChargingSessionStatus GetRandomSessionStatus()
    {
        var statuses = Enum.GetValues(typeof(ChargingSessionStatus));
        return (ChargingSessionStatus)statuses.GetValue(new Random().Next(statuses.Length));
    }

    /// <summary>
    ///     Returns a random status change note
    /// </summary>
    private static string GetRandomStatusNote(ChargerStatus oldStatus, ChargerStatus newStatus)
    {
        if (newStatus == ChargerStatus.OutOfOrder)
        {
            string[] issues = { "Connector damaged", "Display not working", "Power fluctuations", "Communication error", "Payment system failure", "Overheating", "Scheduled maintenance", "Software update required" };

            return issues[new Random().Next(issues.Length)];
        }
        if (oldStatus == ChargerStatus.OutOfOrder && newStatus == ChargerStatus.Available)
        {
            string[] fixes = { "Repaired connector", "Replaced display", "Fixed power supply", "Resolved communication issue", "Updated software", "Completed maintenance", "Restarted system", "Replaced defective parts" };

            return fixes[new Random().Next(fixes.Length)];
        }
        if (newStatus == ChargerStatus.Charging)
        {
            return "Session started";
        }
        if (oldStatus == ChargerStatus.Charging && newStatus == ChargerStatus.Available)
        {
            return "Session completed";
        }

        return string.Empty;
    }

    /// <summary>
    ///     Returns a random first name
    /// </summary>
    private static string GetRandomFirstName()
    {
        string[] firstNames = { "Jan", "Petr", "Martin", "Tomáš", "Pavel", "Jiří", "Lukáš", "David", "Michal", "Josef", "Jaroslav", "Miroslav", "Václav", "Jakub", "Filip", "Marie", "Jana", "Eva", "Hana", "Anna", "Lenka", "Kateřina", "Lucie", "Petra", "Veronika", "Martina", "Jitka", "Tereza", "Monika", "Zdeňka" };

        return firstNames[new Random().Next(firstNames.Length)];
    }

    /// <summary>
    ///     Returns a random last name
    /// </summary>
    private static string GetRandomLastName()
    {
        string[] lastNames = { "Novák", "Svoboda", "Novotný", "Dvořák", "Černý", "Procházka", "Kučera", "Veselý", "Horák", "Němec", "Marek", "Pospíšil", "Hájek", "Jelínek", "Král", "Růžička", "Beneš", "Fiala", "Sedláček", "Doležal", "Zeman", "Kolář", "Navrátil", "Čermák", "Nováková", "Svobodová", "Novotná", "Dvořáková", "Černá", "Procházková", "Kučerová" };

        return lastNames[new Random().Next(lastNames.Length)];
    }

    #endregion

}