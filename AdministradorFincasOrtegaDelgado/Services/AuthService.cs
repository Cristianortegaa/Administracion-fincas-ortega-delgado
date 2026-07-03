using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Helpers;
using AdministradorFincasOrtegaDelgado.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace AdministradorFincasOrtegaDelgado.Services;

public class AuthService(IUserRepository userRepo, IConfiguration config) : IAuthService
{
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await userRepo.GetByEmailAsync(request.Email);
        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            return null;

        var expires = DateTime.UtcNow.AddHours(double.Parse(config["Jwt:ExpiresInHours"]!));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email,          user.Email),
            new Claim(ClaimTypes.Name,           user.Name),
            new Claim(ClaimTypes.Role,           user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:            config["Jwt:Issuer"],
            audience:          config["Jwt:Audience"],
            claims:            claims,
            expires:           expires,
            signingCredentials: creds);

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponseDto(
            Token:    tokenStr,
            ExpiresAt: expires,
            User: new UserInfoDto(user.Id, user.Email, user.Name, user.Role));
    }
}
