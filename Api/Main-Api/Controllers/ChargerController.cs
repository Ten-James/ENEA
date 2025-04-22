using Api.Infrastructure.Models;
using BussinesLogic.Services;
using GeneratedDtos;
using Microsoft.AspNetCore.Mvc;

namespace ENEA.Controllers;

public class ChargerController : Controller
{
    private readonly ILogger<ChargerController> _logger;
    private readonly IService<Charger, ChargerReadDto, ChargerReadDetailDto, ChargerCreateDto, ChargerUpdateDto> _service;

    public ChargerController(
        IService<Charger, ChargerReadDto, ChargerReadDetailDto, ChargerCreateDto, ChargerUpdateDto> service,
        ILogger<ChargerController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IActionResult> Detail(Guid id)
    {
        _logger.LogInformation("Fetching details for charger with ID: {Id}", id);

        var charger = await _service.GetByIdAsync(id);
        if (charger == null)
        {
            _logger.LogWarning("Charger with ID {Id} not found", id);
            return NotFound();
        }

        charger.Events = charger.Events.OrderByDescending(e => e.StartTime).ToList();

        return View(charger);
    }
}