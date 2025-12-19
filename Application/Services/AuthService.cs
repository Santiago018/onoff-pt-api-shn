using Microsoft.IdentityModel.Tokens;
using OnOff.Api.Application.DTOs;
using OnOff.Api.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnOff.Api.Application.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;

    // ID REAL del usuario admin en BD
    private static readonly Guid ADMIN_ID =
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    public LoginResponseDto Login(LoginRequestDto request)
    {
        // Login fake (prueba técnica)
        if (request.Email != "admin@onoff.com" || request.Password != "123456")
            throw new UnauthorizedAccessException("Credenciales inválidas");

        var claims = new[]
        {
            new Claim("userId", ADMIN_ID.ToString()),
            new Claim(ClaimTypes.Email, request.Email)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("ONOFF_SUPER_SECRET_KEY_123456_2025!!")
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new LoginResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token)
        );
    }
}
