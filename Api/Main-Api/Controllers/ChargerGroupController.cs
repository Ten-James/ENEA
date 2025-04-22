using Api.BussinesLogic.Exceptions;
using Api.Infrastructure.Models;
using BussinesLogic.Services;
using Domain;
using GeneratedDtos;
using Main_Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ENEA.Controllers;

public class ChargerGroupController : AuthorizatedController
{
    private readonly ChargerService _chargerService;
    private readonly ILogger<ChargerGroupController> _logger;
    private readonly IService<ChargerGroup, ChargerGroupReadDto, ChargerGroupReadDetailDto, ChargerGroupCreateDto, ChargerGroupUpdateDto> _service;

    public ChargerGroupController(
        IService<ChargerGroup, ChargerGroupReadDto, ChargerGroupReadDetailDto, ChargerGroupCreateDto, ChargerGroupUpdateDto> service,
        ChargerService chargerService,
        ILogger<ChargerGroupController> logger) : base(logger)
    {
        _service = service;
        _chargerService = chargerService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        AuthorizatedGuard();
        _logger.LogInformation("Fetching charger groups: Page {Page}, PageSize {PageSize}", page, pageSize);

        var paginationRequest = new PaginationRequest(page, pageSize);
        var chargerGroups = await _service.GetAllAsync(paginationRequest);

        ViewData["CurrentPage"] = page;
        ViewData["PageSize"] = pageSize;
        ViewData["TotalPages"] = (int)Math.Ceiling((double)chargerGroups.TotalCount / pageSize);

        return View(chargerGroups.Items);
    }

    public async Task<IActionResult> Detail(Guid id)
    {
        AuthorizatedGuard();
        _logger.LogInformation("Fetching details for charger group with ID: {Id}", id);

        var chargerGroup = await _service.GetByIdAsync(id);
        if (chargerGroup == null)
        {
            _logger.LogWarning("Charger group with ID {Id} not found", id);
            return NotFound();
        }

        return View(chargerGroup);
    }

    [HttpGet]
    public IActionResult Add()
    {
        _logger.LogInformation("Accessing Add Charger Group page");
        return View(new ChargerGroupCreateDto());
    }

    [HttpPost]
    public async Task<IActionResult> Add(ChargerGroupCreateDto model)
    {
        AuthorizatedGuard("admin");
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state while adding a new charger group");
            return View(model);
        }

        _logger.LogInformation("Adding a new charger group: {Name}", model.Name);
        await _service.AddAsync(model);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        _logger.LogInformation("Fetching charger group for editing with ID: {Id}", id);

        var chargerGroup = await _service.GetByIdAsync(id);
        if (chargerGroup != null)
        {
            return View(new ChargerGroupUpdateDto { Name = chargerGroup.Name });
        }

        _logger.LogWarning("Charger group with ID {Id} not found", id);
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, ChargerGroupUpdateDto model)
    {
        AuthorizatedGuard("admin");
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for charger group with ID: {Id}", id);
            return View(model);
        }

        _logger.LogInformation("Updating charger group with ID: {Id}", id);
        await _service.UpdateAsync(id, model);

        return RedirectToAction("Detail", new { id });
    }


    [HttpGet]
    public IActionResult AddCharger([FromQuery] Guid groupId)
    {
        _logger.LogInformation("Accessing Add Charger page for group ID: {GroupId}", groupId);

        var model = new ChargerCreateDto { ChargerGroupId = groupId };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddCharger(ChargerCreateDto model)
    {
        AuthorizatedGuard("admin");
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state while adding a new charger to group ID: {GroupId}", model.ChargerGroupId);
            return View(model);
        }

        _logger.LogInformation("Adding a new charger to group ID: {GroupId}", model.ChargerGroupId);
        await _chargerService.AddAsync(model);

        return RedirectToAction("Detail", new { id = model.ChargerGroupId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveCharger(Guid chargerId, Guid groupId)
    {
        await _chargerService.DeleteAsync(chargerId);
        return RedirectToAction("Detail", new { id = groupId });
    }

    [HttpGet]
    public async Task<IActionResult> StartCharging(Guid chargerId, Guid groupId)
    {
        try
        {
            _logger.LogInformation("Starting charging for charger with ID {ChargerId}", chargerId);
            await _chargerService.StartChargingAsync(chargerId, Guid.Parse(UserId));
            return RedirectToAction("Detail", new { id = groupId });
        }
        catch (ChargerInInvalidStateException ex)
        {
            _logger.LogError(ex, "Failed to start charging for charger with ID {ChargerId}", chargerId);
            TempData["ErrorMessage"] = ex.Message;
            TempData["RedirectUrl"] = Url.Action("Detail", new { id = groupId });
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> StopCharging(Guid chargerId, Guid groupId)
    {
        try
        {
            _logger.LogInformation("Stopping charging for charger with ID {ChargerId}", chargerId);
            await _chargerService.StopChargingAsync(chargerId, Guid.Parse(UserId));
            return RedirectToAction("Detail", new { id = groupId });
        }
        catch (ChargerInInvalidStateException ex)
        {
            _logger.LogError(ex, "Failed to stop charging for charger with ID {ChargerId}", chargerId);
            TempData["ErrorMessage"] = ex.Message;
            TempData["RedirectUrl"] = Url.Action("Detail", new { id = groupId });
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> SetToAvailable(Guid chargerId, Guid groupId)
    {
        try
        {
            AuthorizatedGuard("admin");
            _logger.LogInformation("Setting charger with ID {ChargerId} to Available", chargerId);
            await _chargerService.SetToAvailableAsync(chargerId, Guid.Parse(UserId));
            return RedirectToAction("Detail", new { id = groupId });
        }
        catch (ChargerInInvalidStateException ex)
        {
            _logger.LogError(ex, "Failed to set charger with ID {ChargerId} to Available", chargerId);
            TempData["ErrorMessage"] = ex.Message;
            TempData["RedirectUrl"] = Url.Action("Detail", new { id = groupId });
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> SetToOutOfOrder(Guid chargerId, Guid groupId)
    {
        try
        {
            AuthorizatedGuard("admin");
            _logger.LogInformation("Setting charger with ID {ChargerId} to OutOfOrder", chargerId);
            await _chargerService.SetToMaintenanceAsync(chargerId, Guid.Parse(UserId));
            return RedirectToAction("Detail", new { id = groupId });
        }
        catch (ChargerInInvalidStateException ex)
        {
            _logger.LogError(ex, "Failed to set charger with ID {ChargerId} to OutOfOrder", chargerId);
            TempData["ErrorMessage"] = ex.Message;
            TempData["RedirectUrl"] = Url.Action("Detail", new { id = groupId });
            return View("Error");
        }
    }
}