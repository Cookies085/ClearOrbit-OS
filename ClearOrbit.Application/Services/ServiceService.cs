using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Services;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _serviceRepo;
    private readonly IDivisionRepository _divisionRepo;

    public ServiceService(IServiceRepository serviceRepo, IDivisionRepository divisionRepo)
    {
        _serviceRepo = serviceRepo;
        _divisionRepo = divisionRepo;
    }

    public async Task<Result<ServiceResponseDto>> CreateAsync(CreateServiceDto request)
    {
        var errors = ServiceGuard.Validate(request);
        if (errors.Any()) return Result<ServiceResponseDto>.Fail(errors);

        if (await _serviceRepo.ExistsByNameInDivisionAsync(request.Name, request.DivisionId))
            return Result<ServiceResponseDto>.Fail("A service with that name already exists in this division.");

        var division = await _divisionRepo.GetByIdAsync(request.DivisionId);
        if (division is null) return Result<ServiceResponseDto>.Fail("Division not found.");

        var service = new Service
        {
            Name = request.Name.Trim(),
            Description = request.Description,
            BasePrice = request.BasePrice,
            Currency = request.Currency.ToUpperInvariant(),
            Unit = request.Unit,
            DivisionId = request.DivisionId
        };

        await _serviceRepo.AddAsync(service);
        await _serviceRepo.SaveChangesAsync();

        return Result<ServiceResponseDto>.Ok(MapToDto(service, division), "Service created successfully.");
    }

    public async Task<Result<ServiceResponseDto>> GetByIdAsync(Guid id)
    {
        var service = await _serviceRepo.GetByIdAsync(id);
        if (service is null) return Result<ServiceResponseDto>.Fail("Service not found.");
        return Result<ServiceResponseDto>.Ok(MapToDto(service, service.Division));
    }

    public async Task<Result<List<ServiceResponseDto>>> GetAllAsync()
    {
        var services = await _serviceRepo.GetAllAsync();
        var dtos = services.Select(s => MapToDto(s, s.Division)).ToList();
        return Result<List<ServiceResponseDto>>.Ok(dtos);
    }

    public async Task<Result<ServiceResponseDto>> UpdateAsync(Guid id, UpdateServiceDto request)
    {
        var errors = ServiceGuard.Validate(request);
        if (errors.Any()) return Result<ServiceResponseDto>.Fail(errors);

        var service = await _serviceRepo.GetByIdAsync(id);
        if (service is null) return Result<ServiceResponseDto>.Fail("Service not found.");

        var division = await _divisionRepo.GetByIdAsync(request.DivisionId);
        if (division is null) return Result<ServiceResponseDto>.Fail("Division not found.");

        service.Name = request.Name.Trim();
        service.Description = request.Description;
        service.BasePrice = request.BasePrice;
        service.Currency = request.Currency.ToUpperInvariant();
        service.Unit = request.Unit;
        service.DivisionId = request.DivisionId;
        service.IsActive = request.IsActive;

        await _serviceRepo.SaveChangesAsync();
        return Result<ServiceResponseDto>.Ok(MapToDto(service, division), "Service updated successfully.");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var service = await _serviceRepo.GetByIdAsync(id);
        if (service is null) return Result<bool>.Fail("Service not found.");

        service.IsActive = false;
        await _serviceRepo.SaveChangesAsync();
        return Result<bool>.Ok(true, "Service deactivated.");
    }

    public async Task<Result<List<DivisionDto>>> GetDivisionsAsync()
    {
        var divisions = await _divisionRepo.GetAllAsync();
        return Result<List<DivisionDto>>.Ok(divisions.Select(d => new DivisionDto
        {
            Id = d.Id,
            Name = d.Name,
            AccentColor = d.AccentColor
        }).ToList());
    }

    private static ServiceResponseDto MapToDto(Service s, Division? division) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Description = s.Description,
        BasePrice = s.BasePrice,
        Currency = s.Currency,
        Unit = (int)s.Unit,
        UnitName = s.Unit.ToString(),
        DivisionId = s.DivisionId,
        DivisionName = division?.Name ?? string.Empty,
        DivisionAccent = division?.AccentColor ?? "#1E90FF",
        IsActive = s.IsActive,
        CreatedAt = s.CreatedAt
    };
}