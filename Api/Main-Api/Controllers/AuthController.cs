using Api.Infrastructure.Enums;
using Api.Infrastructure.Models;
using Api.Infrastructure.Repositories.User;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Main_Api.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;
    private readonly IUserRepository _userRepository;

    public AuthController(IConfiguration configuration, IUserRepository userRepository, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Registering user with email: {Email}", request.Email);
        var email = request.Email;
        var password = request.Password;
        var userExists = await _userRepository.GetByEmailAsync(email);
        if (userExists != null)
        {
            return StatusCode(409, new { Status = "Error", Message = "User already exists!" });
        }

        var result = await _userRepository.AddAsync(new User { Email = email, Password = password, Name = email });

        return Ok(new { Status = "Success", Message = "User created successfully!" });
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Logging in user with email: {Email}", request.Email);
        var email = request.Email;
        var password = request.Password;
        var user = await _userRepository.GetByEmailAndPasswordAsync(email, password);
        if (user == null)
        {
            return Unauthorized(new { Status = "Error", Message = "Invalid username or password" });
        }


        var authClaims = new List<Claim> { new Claim(ClaimTypes.Name, user.Email), new Claim("id", user.Id.ToString()), new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) };

        var userRoles = new List<string> { "User" };

        if (user.Role == UserRole.Admin)
        {
            userRoles.Add("Admin");
        }

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            _configuration["JWT:ValidIssuer"],
            _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

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