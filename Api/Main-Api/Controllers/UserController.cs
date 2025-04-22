using Api.Infrastructure.Models;
using BussinesLogic.Services;
using Domain;
using GeneratedDtos;
using Main_Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ENEA.Controllers;

public class UserController(
    ILogger<UserController> logger,
    IService<User, UserReadDto, UserReadDetailDto, UserCreateDto, UserUpdateDto> userService) : AuthorizatedController(logger)
{

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        AuthorizatedGuard();
        logger.LogInformation("Fetching paginated user data: Page {Page}, PageSize {PageSize}", page, pageSize);

        var paginationRequest = new PaginationRequest(page, pageSize);

        var users = await userService.GetAllAsync(paginationRequest);

        ViewData["CurrentPage"] = page;
        ViewData["PageSize"] = pageSize;
        ViewData["TotalPages"] = (int)Math.Ceiling((double)users.TotalCount / pageSize);

        return View(users.Items);
    }

    public async Task<IActionResult> Detail(Guid id)
    {
        AuthorizatedGuard();
        logger.LogInformation("Fetching details for user with ID: {Id}", id);

        var user = await userService.GetByIdAsync(id);
        if (user == null)
        {
            logger.LogWarning("User with ID {Id} not found", id);
            return NotFound();
        }

        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        AuthorizatedGuard();
        logger.LogInformation("Fetching user details for editing with ID: {Id}", id);

        var user = await userService.GetByIdAsync(id);
        if (user != null)
        {
            return View(new UserUpdateDto { Name = user.Name, Email = user.Email, Role = user.Role });
        }
        logger.LogWarning("User with ID {Id} not found", id);
        return NotFound();

    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, UserUpdateDto model)
    {
        AuthorizatedGuard();
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for user with ID: {Id}", id);
            return View(model);
        }

        logger.LogInformation("Updating user with ID: {Id}", id);
        await userService.UpdateAsync(id, model);

        return RedirectToAction("Detail", "User", new { id });
    }
}