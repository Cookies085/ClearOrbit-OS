using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IDivisionRepository
{
    Task<List<Division>> GetAllAsync();
    Task<Division?> GetByIdAsync(Guid id);
}