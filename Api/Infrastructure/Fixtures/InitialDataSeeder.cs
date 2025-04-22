using Api.Infrastructure.Enums;
using Api.Infrastructure.Models;
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
    ///     Seeds charger events (status changes and charging sessions) with mock data
    /// </summary>
    private static void SeedChargerEvents(ENEADbContext context)
    {
        var chargers = context.Chargers.ToList();
        var users = context.Users.Take(10).ToList();
        var technicians = context.Users.Skip(10).ToList();
        var events = new List<ChargerEvent>();
        var random = new Random();

        // Generate random events for each charger
        foreach (var charger in chargers)
        {
            // Generate status change history (5-10 events per charger)
            var statusChangeCount = random.Next(5, 11);
            var currentDate = DateTime.UtcNow.AddDays(-30); // Start from 30 days ago
            var previousStatus = ChargerStatus.Available;

            for (var i = 0; i < statusChangeCount; i++)
            {
                // Advance time by 1-48 hours
                currentDate = currentDate.AddHours(random.Next(1, 49));

                // Get a new status different from the previous one
                var newStatus = GetDifferentStatus(previousStatus);

                // Get a random technician
                var technician = technicians[random.Next(technicians.Count)];

                events.Add(new ChargerEvent
                {
                    Id = Guid.NewGuid(),
                    ChargerId = charger.Id,
                    EventType = EventType.StatusChange,
                    StartTime = currentDate,
                    EndTime = currentDate, // For status changes, start and end times are the same
                    OldStatus = previousStatus,
                    NewStatus = newStatus,
                    UserId = technician.Id,
                    Notes = GetRandomStatusNote(previousStatus, newStatus)
                });

                previousStatus = newStatus;
            }

            // Generate charging sessions (10-20 per charger)
            var sessionCount = random.Next(10, 21);
            currentDate = DateTime.UtcNow.AddDays(-30); // Reset to 30 days ago

            for (var i = 0; i < sessionCount; i++)
            {
                // Advance time by 3-24 hours
                currentDate = currentDate.AddHours(random.Next(3, 25));

                // Session duration between 15 minutes and 4 hours
                var durationMinutes = random.Next(15, 241);
                var endTime = currentDate.AddMinutes(durationMinutes);

                // Calculate random energy consumed and price
                var energyConsumed = Math.Round(random.NextDouble() * 50 + 5, 2); // 5-55 kWh
                var totalPrice = Math.Round(energyConsumed * (random.NextDouble() * 2 + 6), 2); // 6-8 per kWh

                var sessionStatus = GetRandomSessionStatus();
                var isCompleted = sessionStatus != ChargingSessionStatus.InProgress;

                // Get a random customer
                var customer = users[random.Next(users.Count)];

                events.Add(new ChargerEvent
                {
                    Id = Guid.NewGuid(),
                    ChargerId = charger.Id,
                    EventType = EventType.ChargingSession,
                    StartTime = currentDate,
                    EndTime = isCompleted ? endTime : null,
                    SessionStatus = sessionStatus,
                    TotalPrice = isCompleted ? totalPrice : null,
                    EnergyConsumed = isCompleted ? energyConsumed : null,
                    IsCompleted = isCompleted,
                    UserId = customer.Id
                });
            }
        }

        context.ChargerEvents.AddRange(events);
        context.SaveChanges();

        // Update chargers' current status based on the last status change
        foreach (var charger in chargers)
        {
            var lastStatusChange = events
                .Where(e => e.ChargerId == charger.Id && e.EventType == EventType.StatusChange).MaxBy(e => e.StartTime);

            if (lastStatusChange != null && lastStatusChange.NewStatus.HasValue)
            {
                charger.CurrentStatus = lastStatusChange.NewStatus.Value;
            }
        }

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