namespace OnOff.Api.Application.DTOs;

public record LoginRequestDto(
    string Email,
    string Password
);
