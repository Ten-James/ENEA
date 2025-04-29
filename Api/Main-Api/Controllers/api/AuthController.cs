using Api.Infrastructure.Models;
using Api.Infrastructure.Repositories.User;
using BussinesLogic.Services;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Main_Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;
    private readonly IUserRepository _userRepository;

    public AuthController(IConfiguration configuration, IUserRepository userRepository, ILogger<AuthController> logger, AuthService authService)
    {
        _configuration = configuration;
        _userRepository = userRepository;
        _logger = logger;
        _authService = authService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Registering user with email: {Email}", request.Email);

        var created = await _authService.Register(request.Email, request.Password);
        if (!created)
        {
            return StatusCode(409, new { Status = "Error", Message = "User already exists!" });
        }

        return Ok(new { Status = "Success", Message = "User created successfully!" });
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Logging in user with email: {Email}", request.Email);

        var token = await _authService.Login(request.Email, request.Password);
        if (token == null)
        {
            return Unauthorized(new { Status = "Error", Message = "Invalid credentials" });
        }

        var strToken = new JwtSecurityTokenHandler().WriteToken(token);
        var response = new LoginResponse(strToken, token.ValidTo);

        return Ok(response);
    }

    [HttpGet]
    [Route("current")]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        _logger.LogInformation("Getting current user");
        var email = User.Identity?.Name;
        var id = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        _logger.LogInformation("Current user email: {Email}, id: {Id}", email, id);
        if (email == null)
        {
            return Unauthorized(new { Status = "Error", Message = "Invalid token" });
        }

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return NotFound(new { Status = "Error", Message = "User not found" });
        }

        return Ok(user);
    }
}