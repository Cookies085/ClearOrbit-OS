using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Software;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Services;

public class FeatureService : IFeatureService
{
    private readonly IFeatureRepository _featureRepo;
    private readonly IDivisionRepository _divisionRepo;
    private readonly IProjectRepository _projectRepo;
    private readonly IClientRepository _clientRepo;

    public FeatureService(
        IFeatureRepository featureRepo,
        IDivisionRepository divisionRepo,
        IProjectRepository projectRepo,
        IClientRepository clientRepo)
    {
        _featureRepo = featureRepo;
        _divisionRepo = divisionRepo;
        _projectRepo = projectRepo;
        _clientRepo = clientRepo;
    }

    public async Task<Result<FeatureResponseDto>> CreateAsync(CreateFeatureDto request)
    {
        var errors = FeatureGuard.Validate(request);
        if (errors.Any()) return Result<FeatureResponseDto>.Fail(errors);

        var division = await _divisionRepo.GetByIdAsync(request.DivisionId);
        if (division is null) return Result<FeatureResponseDto>.Fail("Division not found.");

        Project? project = null;
        if (request.ProjectId.HasValue)
        {
            project = await _projectRepo.GetByIdAsync(request.ProjectId.Value);
            if (project is null) return Result<FeatureResponseDto>.Fail("Project not found.");
        }

        Client? client = null;
        if (request.ClientId.HasValue)
        {
            client = await _clientRepo.GetByIdAsync(request.ClientId.Value);
            if (client is null) return Result<FeatureResponseDto>.Fail("Client not found.");
        }

        var sequence = await _featureRepo.GetNextSequenceAsync();
        var code = FeatureCodeGenerator.Generate(sequence);

        var feature = new Feature
        {
            Code = code,
            Title = request.Title.Trim(),
            Description = request.Description,
            Status = FeatureStatus.Proposed,
            Priority = request.Priority,
            Source = request.Source,
            DivisionId = request.DivisionId,
            ProjectId = request.ProjectId,
            ClientId = request.ClientId,
            AssignedToUserId = request.AssignedToUserId,
            EstimatedEffort = request.EstimatedEffort,
            TargetDate = request.TargetDate
        };

        await _featureRepo.AddAsync(feature);
        await _featureRepo.SaveChangesAsync();

        return Result<FeatureResponseDto>.Ok(
            MapToDto(feature, division, project, client),
            $"Feature {code} created.");
    }

    public async Task<Result<FeatureResponseDto>> GetByIdAsync(Guid id)
    {
        var feature = await _featureRepo.GetByIdAsync(id);
        if (feature is null) return Result<FeatureResponseDto>.Fail("Feature not found.");
        return Result<FeatureResponseDto>.Ok(MapToDto(feature, feature.Division, feature.Project, feature.Client));
    }

    public async Task<Result<List<FeatureResponseDto>>> GetAllAsync()
    {
        var features = await _featureRepo.GetAllAsync();
        return Result<List<FeatureResponseDto>>.Ok(features
            .Select(f => MapToDto(f, f.Division, f.Project, f.Client))
            .ToList());
    }

    public async Task<Result<List<FeatureResponseDto>>> GetByProjectAsync(Guid projectId)
    {
        var features = await _featureRepo.GetByProjectAsync(projectId);
        return Result<List<FeatureResponseDto>>.Ok(features
            .Select(f => MapToDto(f, f.Division, f.Project, f.Client))
            .ToList());
    }

    public async Task<Result<List<FeatureResponseDto>>> GetByDivisionAsync(Guid divisionId)
    {
        var features = await _featureRepo.GetByDivisionAsync(divisionId);
        return Result<List<FeatureResponseDto>>.Ok(features
            .Select(f => MapToDto(f, f.Division, f.Project, f.Client))
            .ToList());
    }

    public async Task<Result<FeatureResponseDto>> UpdateAsync(Guid id, UpdateFeatureDto request)
    {
        var errors = FeatureGuard.Validate(request);
        if (errors.Any()) return Result<FeatureResponseDto>.Fail(errors);

        var feature = await _featureRepo.GetByIdAsync(id);
        if (feature is null) return Result<FeatureResponseDto>.Fail("Feature not found.");

        var division = await _divisionRepo.GetByIdAsync(request.DivisionId);
        if (division is null) return Result<FeatureResponseDto>.Fail("Division not found.");

        Project? project = null;
        if (request.ProjectId.HasValue)
            project = await _projectRepo.GetByIdAsync(request.ProjectId.Value);

        Client? client = null;
        if (request.ClientId.HasValue)
            client = await _clientRepo.GetByIdAsync(request.ClientId.Value);

        feature.Title = request.Title.Trim();
        feature.Description = request.Description;
        feature.Priority = request.Priority;
        feature.Source = request.Source;
        feature.DivisionId = request.DivisionId;
        feature.ProjectId = request.ProjectId;
        feature.ClientId = request.ClientId;
        feature.AssignedToUserId = request.AssignedToUserId;
        feature.EstimatedEffort = request.EstimatedEffort;
        feature.ActualEffort = request.ActualEffort;
        feature.TargetDate = request.TargetDate;

        // Handle status transition
        if (request.Status == FeatureStatus.Shipped && feature.Status != FeatureStatus.Shipped)
            feature.ShippedAt = DateTime.UtcNow;
        else if (request.Status != FeatureStatus.Shipped && feature.Status == FeatureStatus.Shipped)
            feature.ShippedAt = null;

        feature.Status = request.Status;

        await _featureRepo.SaveChangesAsync();
        return Result<FeatureResponseDto>.Ok(
            MapToDto(feature, division, project, client),
            "Feature updated.");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var feature = await _featureRepo.GetByIdAsync(id);
        if (feature is null) return Result<bool>.Fail("Feature not found.");

        feature.IsActive = false;
        await _featureRepo.SaveChangesAsync();
        return Result<bool>.Ok(true, "Feature deleted.");
    }

    private static FeatureResponseDto MapToDto(Feature f, Division? division, Project? project, Client? client) => new()
    {
        Id = f.Id,
        Code = f.Code,
        Title = f.Title,
        Description = f.Description,
        Status = (int)f.Status,
        StatusName = f.Status.ToString(),
        Priority = (int)f.Priority,
        PriorityName = f.Priority.ToString(),
        Source = (int)f.Source,
        SourceName = f.Source.ToString(),
        DivisionId = f.DivisionId,
        DivisionName = division?.Name ?? string.Empty,
        DivisionAccent = division?.AccentColor ?? "#1E90FF",
        ProjectId = f.ProjectId,
        ProjectCode = project?.Code,
        ProjectName = project?.Name,
        ClientId = f.ClientId,
        ClientName = client?.Name,
        AssignedToUserId = f.AssignedToUserId,
        AssignedToName = f.AssignedToUser is null
            ? null
            : $"{f.AssignedToUser.FirstName} {f.AssignedToUser.LastName}",
        EstimatedEffort = f.EstimatedEffort,
        ActualEffort = f.ActualEffort,
        TargetDate = f.TargetDate,
        ShippedAt = f.ShippedAt,
        CreatedAt = f.CreatedAt
    };
}