using Api.Infrastructure.Models;
using BusinessLogic.Interfaces;
using BussinesLogic.Services;
using Domain;
using GeneratedDtos;
using Main_Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ENEA.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    IService<ChargerGroup, ChargerGroupReadDto, ChargerGroupReadDetailDto, ChargerGroupCreateDto, ChargerGroupUpdateDto> groupService,
    AuthService auth,
    IStatService statService) : AuthorizatedController(logger)
{
    private readonly ILogger<HomeController> _logger = logger;

    public async Task<IActionResult> Index()
    {
        if (IsAuthenticated)
        {
            var chargers = await groupService.GetAllAsync(new PaginationRequest(0, 0));

            return View("AuthIndex", chargers.Items);
        }
        logger.LogInformation("Index page accessed");
        return View();
    }

    public IActionResult Register()
    {
        _logger.LogInformation("Register page accessed");
        return View(new LoginRequest("", ""));
    }

    [HttpPost]
    public async Task<IActionResult> Register(LoginRequest model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await auth.Register(model.Email, model.Password);
        if (result)
        {
            _logger.LogInformation("User registered successfully: {Email}", model.Email);
            return RedirectToAction("Login");
        }

        ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
        return View(model);
    }


    public async Task<IActionResult> Logout()
    {
        AuthorizatedGuard();
        logger.LogInformation("Logging out user");
        ClearSession();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Login()
    {
        logger.LogInformation("Login page accessed");
        if (IsAuthenticated)
        {
            logger.LogWarning("User is already authenticated, redirecting to last page");
            var lastPage = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(lastPage))
            {
                return Redirect(lastPage);
            }

            return RedirectToHome();
        }
        return View(new LoginRequest("", ""));
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await auth.Login(model.Email, model.Password);
        if (result is not null)
        {
            logger.LogInformation("User authenticated with token: {Token}", result.Claims);
            Session.SetString("user_email", model.Email);
            SetSession(
                result.Claims.FirstOrDefault(c => c.Type == "id")?.Value,
                string.Join(",", result.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))
            );
            return RedirectToAction("Index");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    public async Task<IActionResult> Stats()
    {
        AuthorizatedGuard();
        _logger.LogInformation("Stats page accessed by user ID: {UserId}", UserId);

        Dictionary<string, MonthStatDto> stats;

        if (UserRoles.Contains("Admin"))
        {
            _logger.LogInformation("Fetching all stats for Admin user ID: {UserId}.", UserId);
            stats = await statService.GetAllStatsAsync();
        }
        else if (Guid.TryParse(UserId, out var userIdGuid))
        {
            _logger.LogInformation("Fetching stats for user ID: {UserId}.", userIdGuid);
            stats = await statService.GetAllStatsAsync(userIdGuid);
        }
        else
        {
            _logger.LogWarning("Could not parse user ID '{UserId}' from session for stats page.", UserId);
            ClearSession();
            return RedirectToAction("Login");
        }

        var orderedStats = stats.OrderBy(kvp => kvp.Key).ToDictionary(keySelector: kvp => kvp.Key, elementSelector: kvp => kvp.Value);

        return View(orderedStats);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}