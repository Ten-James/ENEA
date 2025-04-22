using Api.Infrastructure.Enums;
using Api.Infrastructure.Repositories.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BussinesLogic.Services;

public class AuthService(
    IConfiguration configuration,
    IUserRepository userRepository,
    ILogger<AuthService> logger)
{

    public async Task<JwtSecurityToken?> Login(string email, string password)
    {
        var user = await userRepository.GetByEmailAndPasswordAsync(email, password);
        if (user == null)
        {
            return null;
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

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            configuration["JWT:ValidIssuer"],
            configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}