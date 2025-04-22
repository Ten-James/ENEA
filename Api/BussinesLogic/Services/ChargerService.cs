using Api.BussinesLogic.Exceptions;
using Api.Infrastructure.Enums;
using Api.Infrastructure.Models;
using Api.Infrastructure.Repositories;
using AutoMapper;
using BussinesLogic.Services;
using GeneratedDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class ChargerService(
    IRepository<Charger> chargerRepository,
    IRepository<ChargerEvent> eventRepository,
    ILogger<Service<Charger, ChargerReadDto, ChargerGroupReadDetailDto, ChargerCreateDto, ChargerUpdateDto>> logger,
    IMapper mapper) : Service<Charger, ChargerReadDto, ChargerGroupReadDetailDto, ChargerCreateDto, ChargerUpdateDto>(chargerRepository, logger, mapper)
{
    public async Task StartChargingAsync(Guid chargerId, Guid userId)
    {
        var charger = await chargerRepository.GetByIdAsync(chargerId);
        if (charger == null)
        {
            logger.LogError("Charger with ID {ChargerId} not found.", chargerId);
            throw new EntityNotFoundException(chargerId, "Charger not found.");
        }

        if (charger.CurrentStatus != ChargerStatus.Available)
        {
            logger.LogWarning("Charger with ID {ChargerId} is not available for charging.", chargerId);
            throw new ChargerInInvalidStateException(ChargerStatus.Available, "Charger is not available.");
        }

        var oldStatus = charger.CurrentStatus;
        charger.CurrentStatus = ChargerStatus.Charging;
        await chargerRepository.UpdateAsync(charger);

        // Log the event
        var chargerEvent = new ChargerEvent
        {
            ChargerId = chargerId,
            EventType = EventType.ChargingSession,
            SessionStatus = ChargingSessionStatus.InProgress,
            OldStatus = oldStatus,
            NewStatus = ChargerStatus.Charging,
            StartTime = DateTime.UtcNow,
            UserId = userId,
            Notes = "Started charging"
        };
        await eventRepository.AddAsync(chargerEvent);

        logger.LogInformation("Charger with ID {ChargerId} started charging.", chargerId);
    }

    public async Task StopChargingAsync(Guid chargerId, Guid userId)
    {
        var charger = await chargerRepository.GetByIdAsync(chargerId);
        if (charger == null)
        {
            logger.LogError("Charger with ID {ChargerId} not found.", chargerId);
            throw new KeyNotFoundException("Charger not found.");
        }

        if (charger.CurrentStatus != ChargerStatus.Charging)
        {
            logger.LogWarning("Charger with ID {ChargerId} is not currently charging.", chargerId);
            throw new ChargerInInvalidStateException(ChargerStatus.Charging, "Charger is not charging.");
        }

        var oldStatus = charger.CurrentStatus;
        charger.CurrentStatus = ChargerStatus.Available;
        await chargerRepository.UpdateAsync(charger);

        // Retrieve the active charging event
        var activeEvent = await eventRepository.GetAll()
            .Where(e => e.ChargerId == chargerId && e.SessionStatus == ChargingSessionStatus.InProgress && !e.IsCompleted)
            .OrderByDescending(e => e.StartTime)
            .FirstOrDefaultAsync();

        if (activeEvent == null)
        {
            logger.LogError("No active charging session found for Charger ID {ChargerId}.", chargerId);
            throw new InvalidOperationException("No active charging session found.");
        }

        // Calculate energy consumed and total price
        activeEvent.EndTime = DateTime.UtcNow;
        var durationInHours = (activeEvent.EndTime.Value - activeEvent.StartTime).TotalHours;

        const double energyRatePerHour = 7.5 * 100; // kWh per hour
        const double pricePerKWh = 0.20; // Price per kWh

        activeEvent.EnergyConsumed = durationInHours * energyRatePerHour;
        activeEvent.TotalPrice = activeEvent.EnergyConsumed * pricePerKWh;
        activeEvent.SessionStatus = ChargingSessionStatus.Completed;
        activeEvent.IsCompleted = true;

        await eventRepository.UpdateAsync(activeEvent);

        // Log the event
        var chargerEvent = new ChargerEvent
        {
            ChargerId = chargerId,
            EventType = EventType.ChargingSession,
            OldStatus = oldStatus,
            NewStatus = ChargerStatus.Available,
            StartTime = DateTime.UtcNow,
            UserId = userId,
            Notes = $"Stopped charging. Energy consumed: {activeEvent.EnergyConsumed:F2} kWh, Total price: {activeEvent.TotalPrice:C2}"
        };
        await eventRepository.AddAsync(chargerEvent);

        logger.LogInformation("Charger with ID {ChargerId} stopped charging. Energy consumed: {EnergyConsumed} kWh, Total price: {TotalPrice:C2}.",
            chargerId,
            activeEvent.EnergyConsumed,
            activeEvent.TotalPrice);
    }

    public async Task SetToMaintenanceAsync(Guid chargerId, Guid userId)
    {
        var charger = await chargerRepository.GetByIdAsync(chargerId);
        if (charger == null)
        {
            logger.LogError("Charger with ID {ChargerId} not found.", chargerId);
            throw new EntityNotFoundException(chargerId, "Charger not found.");
        }

        if (charger.CurrentStatus == ChargerStatus.OutOfOrder)
        {
            logger.LogWarning("Charger with ID {ChargerId} is already in maintenance.", chargerId);
            throw new ChargerInInvalidStateException(ChargerStatus.OutOfOrder, "Charger is already in maintenance.");
        }

        var oldStatus = charger.CurrentStatus;
        charger.CurrentStatus = ChargerStatus.OutOfOrder;
        await chargerRepository.UpdateAsync(charger);

        // Log the event
        var chargerEvent = new ChargerEvent
        {
            ChargerId = chargerId,
            EventType = EventType.StatusChange,
            OldStatus = oldStatus,
            NewStatus = ChargerStatus.OutOfOrder,
            StartTime = DateTime.UtcNow,
            UserId = userId,
            Notes = "Set to maintenance"
        };
        await eventRepository.AddAsync(chargerEvent);

        logger.LogInformation("Charger with ID {ChargerId} set to maintenance.", chargerId);
    }

    public async Task SetToAvailableAsync(Guid chargerId, Guid userId)
    {
        var charger = await chargerRepository.GetByIdAsync(chargerId);
        if (charger == null)
        {
            logger.LogError("Charger with ID {ChargerId} not found.", chargerId);
            throw new EntityNotFoundException(chargerId, "Charger not found.");
        }

        if (charger.CurrentStatus != ChargerStatus.OutOfOrder)
        {
            logger.LogWarning("Charger with ID {ChargerId} is not in maintenance.", chargerId);
            throw new ChargerInInvalidStateException(ChargerStatus.OutOfOrder, "Charger is not in maintenance.");
        }

        var oldStatus = charger.CurrentStatus;
        charger.CurrentStatus = ChargerStatus.Available;
        await chargerRepository.UpdateAsync(charger);

        // Log the event
        var chargerEvent = new ChargerEvent
        {
            ChargerId = chargerId,
            EventType = EventType.StatusChange,
            OldStatus = oldStatus,
            NewStatus = ChargerStatus.Available,
            StartTime = DateTime.UtcNow,
            UserId = userId,
            Notes = "Set to available"
        };
        await eventRepository.AddAsync(chargerEvent);

        logger.LogInformation("Charger with ID {ChargerId} set to available.", chargerId);
    }
}