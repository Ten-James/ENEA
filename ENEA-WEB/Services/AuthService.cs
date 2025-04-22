using Generated.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ENEA.WEB.Services;

public class AuthService
{
    private readonly MyApiClient _apiClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;

    public AuthService(MyApiClient apiClient, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<IActionResult> login(LoginRequest request)
    {
        _logger.LogInformation("Login attempt with email: {Email}", request.Email);
        var result = await _apiClient.LoginAsync(request);
        if (result is not null)
        {
            var claims = new List<Claim> { new Claim("JwtToken", result.Token) };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3) // Set expiration for the cookie
            };

            var context = _httpContextAccessor.HttpContext;

            _logger.LogInformation("User authenticated with token: {Token}", result.Token);
            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);


        }
        return new UnauthorizedResult();
    }
}