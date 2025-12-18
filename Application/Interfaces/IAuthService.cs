using OnOff.Api.Application.DTOs;

namespace OnOff.Api.Application.Interfaces;

public interface IAuthService
{
    LoginResponseDto Login(LoginRequestDto request);
}
