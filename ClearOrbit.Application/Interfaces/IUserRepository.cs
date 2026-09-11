using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}