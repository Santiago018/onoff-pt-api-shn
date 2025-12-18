using Microsoft.AspNetCore.Mvc;
using OnOff.Api.Application.DTOs;
using OnOff.Api.Application.Interfaces;

namespace OnOff.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto request)
    {
        _logger.LogInformation(
            "Intento de login para el usuario {Email}",
            request.Email
        );

        try
        {
            var result = _authService.Login(request);

            _logger.LogInformation(
                "Login exitoso para el usuario {Email}",
                request.Email
            );

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(
                "Login fallido para el usuario {Email}. Motivo: {Message}",
                request.Email,
                ex.Message
            );

            return Unauthorized(new
            {
                message = "Credenciales inválidas"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error inesperado durante el login del usuario {Email}",
                request.Email
            );

            return StatusCode(500, new
            {
                message = "Error interno del servidor"
            });
        }
    }
}
