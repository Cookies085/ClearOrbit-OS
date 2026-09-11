using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Auth;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.ValueObjects;

namespace ClearOrbit.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthenticationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        // 1. Guard: business rules
        var errors = UserGuard.Validate(request);
        if (errors.Any())
            return Result<AuthResponseDto>.Fail(errors);

        // 2. Rule: no duplicate emails
        if (await _userRepository.ExistsAsync(request.Email))
            return Result<AuthResponseDto>.Fail("A user with that email already exists.");

        // 3. Build the entity
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = new EmailAddress(request.Email),
            PasswordHash = _passwordHasher.Hash(request.Password),
            SystemPrefix = "CO"
        };

        // 4. Persist
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        // 5. Return a token so they're logged in immediately
        var token = _tokenGenerator.GenerateToken(user);
        return Result<AuthResponseDto>.Ok(new AuthResponseDto
        {
            UserId = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email.Value,
            Token = token.Token,
            ExpiresAt = token.ExpiresAt
        }, "Registered successfully.");
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        // 1. Find user
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
            return Result<AuthResponseDto>.Fail("Invalid email or password.");

        // 2. Verify password
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponseDto>.Fail("Invalid email or password.");

        // 3. Generate token
        var token = _tokenGenerator.GenerateToken(user);

        return Result<AuthResponseDto>.Ok(new AuthResponseDto
        {
            UserId = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email.Value,
            Token = token.Token,
            ExpiresAt = token.ExpiresAt
        }, "Logged in successfully.");
    }
}