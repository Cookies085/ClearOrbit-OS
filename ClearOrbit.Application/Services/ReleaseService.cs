using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Software;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Services;

public class ReleaseService : IReleaseService
{
    private readonly IReleaseRepository _releaseRepo;
    private readonly IDivisionRepository _divisionRepo;
    private readonly IProjectRepository _projectRepo;
    private readonly IFeatureRepository _featureRepo;

    public ReleaseService(
        IReleaseRepository releaseRepo,
        IDivisionRepository divisionRepo,
        IProjectRepository projectRepo,
        IFeatureRepository featureRepo)
    {
        _releaseRepo = releaseRepo;
        _divisionRepo = divisionRepo;
        _projectRepo = projectRepo;
        _featureRepo = featureRepo;
    }

    public async Task<Result<ReleaseResponseDto>> CreateAsync(CreateReleaseDto request)
    {
        var errors = ReleaseGuard.Validate(request);
        if (errors.Any()) return Result<ReleaseResponseDto>.Fail(errors);

        var division = await _divisionRepo.GetByIdAsync(request.DivisionId);
        if (division is null) return Result<ReleaseResponseDto>.Fail("Division not found.");

        Project? project = null;
        if (request.ProjectId.HasValue)
        {
            project = await _projectRepo.GetByIdAsync(request.ProjectId.Value);
            if (project is null) return Result<ReleaseResponseDto>.Fail("Project not found.");
        }

        if (await _releaseRepo.VersionExistsAsync(request.Version.Trim(), request.ProjectId))
            return Result<ReleaseResponseDto>.Fail(
                "A release with that version already exists for this project.");

        var sequence = await _releaseRepo.GetNextSequenceAsync();
        var code = ReleaseCodeGenerator.Generate(sequence);

        var release = new Release
        {
            Code = code,
            Version = request.Version.Trim(),
            Name = request.Name.Trim(),
            Description = request.Description,
            ReleaseNotes = request.ReleaseNotes,
            Status = ReleaseStatus.Planning,
            DivisionId = request.DivisionId,
            ProjectId = request.ProjectId,
            PlannedDate = request.PlannedDate,
            ReleaseManagerUserId = request.ReleaseManagerUserId
        };

        await _releaseRepo.AddAsync(release);
        await _releaseRepo.SaveChangesAsync();

        // Assign features
        if (request.FeatureIds.Any())
        {
            foreach (var featureId in request.FeatureIds)
            {
                var feature = await _featureRepo.GetByIdAsync(featureId);
                if (feature is null) continue;
                feature.ReleaseId = release.Id;
            }
            await _releaseRepo.SaveChangesAsync();
        }

        // Reload with features
        var reloaded = await _releaseRepo.GetByIdAsync(release.Id);
        return Result<ReleaseResponseDto>.Ok(
            MapToDto(reloaded!, division, project),
            $"Release {code} created.");
    }

    public async Task<Result<ReleaseResponseDto>> GetByIdAsync(Guid id)
    {
        var release = await _releaseRepo.GetByIdAsync(id);
        if (release is null) return Result<ReleaseResponseDto>.Fail("Release not found.");
        return Result<ReleaseResponseDto>.Ok(MapToDto(release, release.Division, release.Project));
    }

    public async Task<Result<List<ReleaseResponseDto>>> GetAllAsync()
    {
        var releases = await _releaseRepo.GetAllAsync();
        return Result<List<ReleaseResponseDto>>.Ok(releases
            .Select(r => MapToDto(r, r.Division, r.Project))
            .ToList());
    }

    public async Task<Result<List<ReleaseResponseDto>>> GetByProjectAsync(Guid projectId)
    {
        var releases = await _releaseRepo.GetByProjectAsync(projectId);
        return Result<List<ReleaseResponseDto>>.Ok(releases
            .Select(r => MapToDto(r, r.Division, r.Project))
            .ToList());
    }

    public async Task<Result<ReleaseResponseDto>> UpdateAsync(Guid id, UpdateReleaseDto request)
    {
        var errors = ReleaseGuard.Validate(request);
        if (errors.Any()) return Result<ReleaseResponseDto>.Fail(errors);

        var release = await _releaseRepo.GetByIdAsync(id);
        if (release is null) return Result<ReleaseResponseDto>.Fail("Release not found.");

        var division = await _divisionRepo.GetByIdAsync(request.DivisionId);
        if (division is null) return Result<ReleaseResponseDto>.Fail("Division not found.");

        Project? project = null;
        if (request.ProjectId.HasValue)
        {
            project = await _projectRepo.GetByIdAsync(request.ProjectId.Value);
            if (project is null) return Result<ReleaseResponseDto>.Fail("Project not found.");
        }

        if (await _releaseRepo.VersionExistsAsync(request.Version.Trim(), request.ProjectId, id))
            return Result<ReleaseResponseDto>.Fail(
                "Another release already uses that version for this project.");

        var wasReleased = release.Status == ReleaseStatus.Released;
        var isNowReleased = request.Status == ReleaseStatus.Released;

        release.Version = request.Version.Trim();
        release.Name = request.Name.Trim();
        release.Description = request.Description;
        release.ReleaseNotes = request.ReleaseNotes;
        release.Status = request.Status;
        release.DivisionId = request.DivisionId;
        release.ProjectId = request.ProjectId;
        release.PlannedDate = request.PlannedDate;
        release.ReleaseManagerUserId = request.ReleaseManagerUserId;

        if (isNowReleased && !wasReleased)
            release.ReleasedAt = DateTime.UtcNow;
        else if (!isNowReleased && wasReleased)
            release.ReleasedAt = null;

        // Sync feature assignments
        var currentFeatureIds = release.Features.Select(f => f.Id).ToHashSet();
        var requestedFeatureIds = request.FeatureIds.ToHashSet();

        // Unassign features that were removed
        foreach (var feature in release.Features.Where(f => !requestedFeatureIds.Contains(f.Id)).ToList())
        {
            feature.ReleaseId = null;
        }

        // Assign new features
        foreach (var featureId in requestedFeatureIds.Except(currentFeatureIds))
        {
            var feature = await _featureRepo.GetByIdAsync(featureId);
            if (feature is null) continue;
            feature.ReleaseId = release.Id;

            // If release is being marked released, mark features as shipped too
            if (isNowReleased && feature.Status != FeatureStatus.Shipped)
            {
                feature.Status = FeatureStatus.Shipped;
                feature.ShippedAt = DateTime.UtcNow;
            }
        }

        // If already released, mark any newly added features as shipped
        if (isNowReleased)
        {
            foreach (var featureId in requestedFeatureIds)
            {
                var feature = await _featureRepo.GetByIdAsync(featureId);
                if (feature is null) continue;
                if (feature.Status != FeatureStatus.Shipped)
                {
                    feature.Status = FeatureStatus.Shipped;
                    feature.ShippedAt = DateTime.UtcNow;
                }
            }
        }

        await _releaseRepo.SaveChangesAsync();

        var reloaded = await _releaseRepo.GetByIdAsync(id);
        return Result<ReleaseResponseDto>.Ok(
            MapToDto(reloaded!, division, project),
            "Release updated.");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var release = await _releaseRepo.GetByIdAsync(id);
        if (release is null) return Result<bool>.Fail("Release not found.");

        // Unlink features
        foreach (var feature in release.Features.ToList())
            feature.ReleaseId = null;

        release.IsActive = false;
        release.Status = ReleaseStatus.Cancelled;
        await _releaseRepo.SaveChangesAsync();

        return Result<bool>.Ok(true, "Release cancelled.");
    }

    private static ReleaseResponseDto MapToDto(Release r, Division? division, Project? project) => new()
    {
        Id = r.Id,
        Code = r.Code,
        Version = r.Version,
        Name = r.Name,
        Description = r.Description,
        ReleaseNotes = r.ReleaseNotes,
        Status = (int)r.Status,
        StatusName = r.Status.ToString(),
        DivisionId = r.DivisionId,
        DivisionName = division?.Name ?? string.Empty,
        DivisionAccent = division?.AccentColor ?? "#1E90FF",
        ProjectId = r.ProjectId,
        ProjectCode = project?.Code,
        ProjectName = project?.Name,
        PlannedDate = r.PlannedDate,
        ReleasedAt = r.ReleasedAt,
        ReleaseManagerUserId = r.ReleaseManagerUserId,
        ReleaseManagerName = r.ReleaseManagerUser is null
            ? null
            : $"{r.ReleaseManagerUser.FirstName} {r.ReleaseManagerUser.LastName}",
        FeatureCount = r.Features?.Count ?? 0,
        Features = r.Features?.Select(f => new FeatureSummaryDto
        {
            Id = f.Id,
            Code = f.Code,
            Title = f.Title,
            StatusName = f.Status.ToString()
        }).ToList() ?? new List<FeatureSummaryDto>(),
        CreatedAt = r.CreatedAt
    };
}