using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Software;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Services;

public class BugService : IBugService
{
    private readonly IBugRepository _bugRepo;
    private readonly IDivisionRepository _divisionRepo;
    private readonly IProjectRepository _projectRepo;
    private readonly IFeatureRepository _featureRepo;

    public BugService(
        IBugRepository bugRepo,
        IDivisionRepository divisionRepo,
        IProjectRepository projectRepo,
        IFeatureRepository featureRepo)
    {
        _bugRepo = bugRepo;
        _divisionRepo = divisionRepo;
        _projectRepo = projectRepo;
        _featureRepo = featureRepo;
    }

    public async Task<Result<BugResponseDto>> CreateAsync(CreateBugDto request, Guid? reportedByUserId)
    {
        var errors = BugGuard.Validate(request);
        if (errors.Any()) return Result<BugResponseDto>.Fail(errors);

        var division = await _divisionRepo.GetByIdAsync(request.DivisionId);
        if (division is null) return Result<BugResponseDto>.Fail("Division not found.");

        Project? project = null;
        if (request.ProjectId.HasValue)
        {
            project = await _projectRepo.GetByIdAsync(request.ProjectId.Value);
            if (project is null) return Result<BugResponseDto>.Fail("Project not found.");
        }

        Feature? feature = null;
        if (request.FeatureId.HasValue)
        {
            feature = await _featureRepo.GetByIdAsync(request.FeatureId.Value);
            if (feature is null) return Result<BugResponseDto>.Fail("Feature not found.");
        }

        var sequence = await _bugRepo.GetNextSequenceAsync();
        var code = BugCodeGenerator.Generate(sequence);

        var bug = new Bug
        {
            Code = code,
            Title = request.Title.Trim(),
            Description = request.Description,
            StepsToReproduce = request.StepsToReproduce,
            ExpectedBehavior = request.ExpectedBehavior,
            ActualBehavior = request.ActualBehavior,
            Status = BugStatus.Open,
            Severity = request.Severity,
            Priority = request.Priority,
            DivisionId = request.DivisionId,
            ProjectId = request.ProjectId,
            FeatureId = request.FeatureId,
            AssignedToUserId = request.AssignedToUserId,
            ReportedByUserId = reportedByUserId,
            ReportedAt = DateTime.UtcNow
        };

        await _bugRepo.AddAsync(bug);
        await _bugRepo.SaveChangesAsync();

        return Result<BugResponseDto>.Ok(
            MapToDto(bug, division, project, feature),
            $"Bug {code} reported.");
    }

    public async Task<Result<BugResponseDto>> GetByIdAsync(Guid id)
    {
        var bug = await _bugRepo.GetByIdAsync(id);
        if (bug is null) return Result<BugResponseDto>.Fail("Bug not found.");
        return Result<BugResponseDto>.Ok(MapToDto(bug, bug.Division, bug.Project, bug.Feature));
    }

    public async Task<Result<List<BugResponseDto>>> GetAllAsync()
    {
        var bugs = await _bugRepo.GetAllAsync();
        return Result<List<BugResponseDto>>.Ok(bugs
            .Select(b => MapToDto(b, b.Division, b.Project, b.Feature))
            .ToList());
    }

    public async Task<Result<List<BugResponseDto>>> GetByProjectAsync(Guid projectId)
    {
        var bugs = await _bugRepo.GetByProjectAsync(projectId);
        return Result<List<BugResponseDto>>.Ok(bugs
            .Select(b => MapToDto(b, b.Division, b.Project, b.Feature))
            .ToList());
    }

    public async Task<Result<List<BugResponseDto>>> GetByFeatureAsync(Guid featureId)
    {
        var bugs = await _bugRepo.GetByFeatureAsync(featureId);
        return Result<List<BugResponseDto>>.Ok(bugs
            .Select(b => MapToDto(b, b.Division, b.Project, b.Feature))
            .ToList());
    }

    public async Task<Result<List<BugResponseDto>>> GetByDivisionAsync(Guid divisionId)
    {
        var bugs = await _bugRepo.GetByDivisionAsync(divisionId);
        return Result<List<BugResponseDto>>.Ok(bugs
            .Select(b => MapToDto(b, b.Division, b.Project, b.Feature))
            .ToList());
    }

    public async Task<Result<BugResponseDto>> UpdateAsync(Guid id, UpdateBugDto request)
    {
        var errors = BugGuard.Validate(request);
        if (errors.Any()) return Result<BugResponseDto>.Fail(errors);

        var bug = await _bugRepo.GetByIdAsync(id);
        if (bug is null) return Result<BugResponseDto>.Fail("Bug not found.");

        var division = await _divisionRepo.GetByIdAsync(request.DivisionId);
        if (division is null) return Result<BugResponseDto>.Fail("Division not found.");

        Project? project = null;
        if (request.ProjectId.HasValue)
            project = await _projectRepo.GetByIdAsync(request.ProjectId.Value);

        Feature? feature = null;
        if (request.FeatureId.HasValue)
            feature = await _featureRepo.GetByIdAsync(request.FeatureId.Value);

        bug.Title = request.Title.Trim();
        bug.Description = request.Description;
        bug.StepsToReproduce = request.StepsToReproduce;
        bug.ExpectedBehavior = request.ExpectedBehavior;
        bug.ActualBehavior = request.ActualBehavior;
        bug.Severity = request.Severity;
        bug.Priority = request.Priority;
        bug.DivisionId = request.DivisionId;
        bug.ProjectId = request.ProjectId;
        bug.FeatureId = request.FeatureId;
        bug.AssignedToUserId = request.AssignedToUserId;

        // Handle status transitions
        if (request.Status == BugStatus.Fixed && bug.Status != BugStatus.Fixed)
            bug.FixedAt = DateTime.UtcNow;
        else if (request.Status != BugStatus.Fixed && bug.Status == BugStatus.Fixed)
            bug.FixedAt = null;

        if (request.Status == BugStatus.Verified && bug.Status != BugStatus.Verified)
            bug.VerifiedAt = DateTime.UtcNow;
        else if (request.Status != BugStatus.Verified && bug.Status == BugStatus.Verified)
            bug.VerifiedAt = null;

        bug.Status = request.Status;

        await _bugRepo.SaveChangesAsync();
        return Result<BugResponseDto>.Ok(
            MapToDto(bug, division, project, feature),
            "Bug updated.");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var bug = await _bugRepo.GetByIdAsync(id);
        if (bug is null) return Result<bool>.Fail("Bug not found.");

        bug.IsActive = false;
        await _bugRepo.SaveChangesAsync();
        return Result<bool>.Ok(true, "Bug deleted.");
    }

    private static BugResponseDto MapToDto(Bug b, Division? division, Project? project, Feature? feature) => new()
    {
        Id = b.Id,
        Code = b.Code,
        Title = b.Title,
        Description = b.Description,
        StepsToReproduce = b.StepsToReproduce,
        ExpectedBehavior = b.ExpectedBehavior,
        ActualBehavior = b.ActualBehavior,
        Status = (int)b.Status,
        StatusName = b.Status.ToString(),
        Severity = (int)b.Severity,
        SeverityName = b.Severity.ToString(),
        Priority = (int)b.Priority,
        PriorityName = b.Priority.ToString(),
        DivisionId = b.DivisionId,
        DivisionName = division?.Name ?? string.Empty,
        DivisionAccent = division?.AccentColor ?? "#1E90FF",
        ProjectId = b.ProjectId,
        ProjectCode = project?.Code,
        ProjectName = project?.Name,
        FeatureId = b.FeatureId,
        FeatureCode = feature?.Code,
        FeatureTitle = feature?.Title,
        AssignedToUserId = b.AssignedToUserId,
        AssignedToName = b.AssignedToUser is null
            ? null
            : $"{b.AssignedToUser.FirstName} {b.AssignedToUser.LastName}",
        ReportedAt = b.ReportedAt,
        FixedAt = b.FixedAt,
        VerifiedAt = b.VerifiedAt,
        CreatedAt = b.CreatedAt
    };
}