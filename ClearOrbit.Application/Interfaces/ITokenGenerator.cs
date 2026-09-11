using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface ITokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}