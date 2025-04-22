using Api.BussinesLogic.Exceptions;
using Api.Infrastructure.Models;
using BussinesLogic.Services;
using GeneratedDtos;
using Main_Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/charger")]
public class ChargerController : BaseApiController<Charger, ChargerReadDto, ChargerReadDetailDto, ChargerCreateDto, ChargerUpdateDto>
{
    private readonly ChargerService _chargerService;
    private readonly ILogger<ChargerController> _logger;

    public ChargerController(ChargerService chargerService, IService<Charger, ChargerReadDto, ChargerReadDetailDto, ChargerCreateDto, ChargerUpdateDto> service, ILogger<ChargerController> logger): base(service)
    {
        _chargerService = chargerService;
        _logger = logger;
    }

    [HttpPost("{chargerId}/start-charging")]
    [Authorize]
    public async Task<IActionResult> StartCharging(Guid chargerId)
    {
        try
        {
            _logger.LogInformation("Starting charging for charger with ID {ChargerId}", chargerId);
            await _chargerService.StartChargingAsync(chargerId, UserId);
            return Ok("Charging started successfully.");
        }
        catch (ChargerInInvalidStateException ex)
        {
            _logger.LogError(ex, "Failed to start charging for charger with ID {ChargerId}", chargerId);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{chargerId}/stop-charging")]
    [Authorize]
    public async Task<IActionResult> StopCharging(Guid chargerId)
    {
        try
        {
            _logger.LogInformation("Stopping charging for charger with ID {ChargerId}", chargerId);
            await _chargerService.StopChargingAsync(chargerId, UserId);
            return Ok("Charging stopped successfully.");
        }
        catch (ChargerInInvalidStateException ex)
        {
            _logger.LogError(ex, "Failed to stop charging for charger with ID {ChargerId}", chargerId);
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{chargerId}/set-to-available")]
    [Authorize]
    public async Task<IActionResult> SetToAvailable(Guid chargerId)
    {
        try
        {
            _logger.LogInformation("Setting charger with ID {ChargerId} to Available", chargerId);
            await _chargerService.SetToAvailableAsync(chargerId, UserId);
            return Ok("Charger set to available.");
        }
        catch (ChargerInInvalidStateException ex)
        {
            _logger.LogError(ex, "Failed to set charger with ID {ChargerId} to Available", chargerId);
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{chargerId}/set-out-of-order")]
    [Authorize]
    public async Task<IActionResult> SetToOutOfOrder(Guid chargerId)
    {
        try
        {
            _logger.LogInformation("Setting charger with ID {ChargerId} to OutOfOrder", chargerId);
            await _chargerService.SetToMaintenanceAsync(chargerId, UserId);
            return Ok("Charger set to out of order.");
        }
        catch (ChargerInInvalidStateException ex)
        {
            _logger.LogError(ex, "Failed to set charger with ID {ChargerId} to OutOfOrder", chargerId);
            return BadRequest(ex.Message);
        }
    }
}