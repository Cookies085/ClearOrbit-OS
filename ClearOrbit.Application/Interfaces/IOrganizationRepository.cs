using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Interfaces;

public interface IOrganizationRepository
{
    Task<Organization?> GetDefaultAsync();
}