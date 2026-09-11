using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Projects;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepo;
    private readonly IClientRepository _clientRepo;
    private readonly IDivisionRepository _divisionRepo;
    private readonly IServiceRepository _serviceRepo;

    public ProjectService(
        IProjectRepository projectRepo,
        IClientRepository clientRepo,
        IDivisionRepository divisionRepo,
        IServiceRepository serviceRepo)
    {
        _projectRepo = projectRepo;
        _clientRepo = clientRepo;
        _divisionRepo = divisionRepo;
        _serviceRepo = serviceRepo;
    }

    public async Task<Result<ProjectResponseDto>> CreateAsync(CreateProjectDto request)
    {
        var errors = ProjectGuard.Validate(request);
        if (errors.Any()) return Result<ProjectResponseDto>.Fail(errors);

        var division = await _divisionRepo.GetByIdAsync(request.DivisionId);
        if (division is null) return Result<ProjectResponseDto>.Fail("Delivering division not found.");

        Client? client = null;
        if (request.ClientId.HasValue)
        {
            client = await _clientRepo.GetByIdAsync(request.ClientId.Value);
            if (client is null) return Result<ProjectResponseDto>.Fail("Client not found.");
        }

        Service? service = null;
        if (request.ServiceId.HasValue)
        {
            service = await _serviceRepo.GetByIdAsync(request.ServiceId.Value);
            if (service is null) return Result<ProjectResponseDto>.Fail("Service not found.");
        }

        Division? requestingDivision = null;
        if (request.RequestingDivisionId.HasValue)
        {
            requestingDivision = await _divisionRepo.GetByIdAsync(request.RequestingDivisionId.Value);
            if (requestingDivision is null) return Result<ProjectResponseDto>.Fail("Requesting division not found.");
        }

        var sequence = await _projectRepo.GetNextSequenceForDivisionAsync(request.DivisionId);
        var code = ProjectCodeGenerator.Generate(division, sequence);

        var project = new Project
        {
            Code = code,
            Name = request.Name.Trim(),
            Description = request.Description,
            Type = request.Type,
            Status = ProjectStatus.Draft,
            ClientId = request.ClientId,
            DivisionId = request.DivisionId,
            ServiceId = request.ServiceId,
            RequestingDivisionId = request.RequestingDivisionId,
            Budget = request.Budget,
            Currency = request.Currency.ToUpperInvariant(),
            StartDate = request.StartDate,
            DueDate = request.DueDate
        };

        await _projectRepo.AddAsync(project);
        await _projectRepo.SaveChangesAsync();

        return Result<ProjectResponseDto>.Ok(
            MapToDto(project, client, division, service, requestingDivision),
            "Project created successfully.");
    }

    public async Task<Result<ProjectResponseDto>> GetByIdAsync(Guid id)
    {
        var project = await _projectRepo.GetByIdAsync(id);
        if (project is null) return Result<ProjectResponseDto>.Fail("Project not found.");

        return Result<ProjectResponseDto>.Ok(MapToDto(
            project, project.Client, project.Division, project.Service, project.RequestingDivision));
    }

    public async Task<Result<List<ProjectResponseDto>>> GetAllAsync()
    {
        var projects = await _projectRepo.GetAllAsync();
        return Result<List<ProjectResponseDto>>.Ok(projects
            .Select(p => MapToDto(p, p.Client, p.Division, p.Service, p.RequestingDivision))
            .ToList());
    }

    public async Task<Result<ProjectResponseDto>> UpdateAsync(Guid id, UpdateProjectDto request)
    {
        var errors = ProjectGuard.Validate(request);
        if (errors.Any()) return Result<ProjectResponseDto>.Fail(errors);

        var project = await _projectRepo.GetByIdAsync(id);
        if (project is null) return Result<ProjectResponseDto>.Fail("Project not found.");

        var division = await _divisionRepo.GetByIdAsync(request.DivisionId);
        if (division is null) return Result<ProjectResponseDto>.Fail("Delivering division not found.");

        Client? client = null;
        if (request.ClientId.HasValue)
        {
            client = await _clientRepo.GetByIdAsync(request.ClientId.Value);
            if (client is null) return Result<ProjectResponseDto>.Fail("Client not found.");
        }

        Service? service = null;
        if (request.ServiceId.HasValue)
            service = await _serviceRepo.GetByIdAsync(request.ServiceId.Value);

        Division? requestingDivision = null;
        if (request.RequestingDivisionId.HasValue)
            requestingDivision = await _divisionRepo.GetByIdAsync(request.RequestingDivisionId.Value);

        project.Name = request.Name.Trim();
        project.Description = request.Description;
        project.Status = request.Status;
        project.ClientId = request.ClientId;
        project.DivisionId = request.DivisionId;
        project.ServiceId = request.ServiceId;
        project.RequestingDivisionId = request.RequestingDivisionId;
        project.Budget = request.Budget;
        project.Currency = request.Currency.ToUpperInvariant();
        project.StartDate = request.StartDate;
        project.DueDate = request.DueDate;

        if (request.Status == ProjectStatus.Completed && project.CompletedAt is null)
            project.CompletedAt = DateTime.UtcNow;

        await _projectRepo.SaveChangesAsync();

        return Result<ProjectResponseDto>.Ok(
            MapToDto(project, client, division, service, requestingDivision),
            "Project updated successfully.");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var project = await _projectRepo.GetByIdAsync(id);
        if (project is null) return Result<bool>.Fail("Project not found.");

        project.IsActive = false;
        await _projectRepo.SaveChangesAsync();
        return Result<bool>.Ok(true, "Project deactivated.");
    }

    private static ProjectResponseDto MapToDto(
        Project p, Client? client, Division division, Service? service, Division? requestingDivision) => new()
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Description = p.Description,
            Type = (int)p.Type,
            TypeName = p.Type.ToString(),
            Status = (int)p.Status,
            StatusName = p.Status.ToString(),
            ClientId = p.ClientId,
            ClientName = client?.Name,
            DivisionId = p.DivisionId,
            DivisionName = division.Name,
            DivisionAccent = division.AccentColor,
            ServiceId = p.ServiceId,
            ServiceName = service?.Name,
            RequestingDivisionId = p.RequestingDivisionId,
            RequestingDivisionName = requestingDivision?.Name,
            Budget = p.Budget,
            Currency = p.Currency,
            StartDate = p.StartDate,
            DueDate = p.DueDate,
            CompletedAt = p.CompletedAt,
            CreatedAt = p.CreatedAt
        };
}