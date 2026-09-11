using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Services;

namespace ClearOrbit.Application.Interfaces;

public interface IServiceService
{
    Task<Result<ServiceResponseDto>> CreateAsync(CreateServiceDto request);
    Task<Result<ServiceResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<ServiceResponseDto>>> GetAllAsync();
    Task<Result<ServiceResponseDto>> UpdateAsync(Guid id, UpdateServiceDto request);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<List<DivisionDto>>> GetDivisionsAsync();
}

public class DivisionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AccentColor { get; set; } = string.Empty;
}