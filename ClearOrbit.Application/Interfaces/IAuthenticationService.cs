using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Auth;

namespace ClearOrbit.Application.Interfaces;

public interface IAuthenticationService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request);
}