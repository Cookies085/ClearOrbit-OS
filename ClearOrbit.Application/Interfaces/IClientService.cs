using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Clients;

namespace ClearOrbit.Application.Interfaces;

public interface IClientService
{
    Task<Result<ClientResponseDto>> CreateAsync(CreateClientDto request);
    Task<Result<ClientResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<ClientResponseDto>>> GetAllAsync();
    Task<Result<ClientResponseDto>> UpdateAsync(Guid id, UpdateClientDto request);
    Task<Result<bool>> DeleteAsync(Guid id);
}