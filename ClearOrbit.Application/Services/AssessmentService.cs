using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Academy;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Services;

public class AssessmentService : IAssessmentService
{
    private readonly IAssessmentRepository _assessmentRepo;
    private readonly IResultRepository _resultRepo;
    private readonly IClassRepository _classRepo;

    public AssessmentService(
        IAssessmentRepository assessmentRepo,
        IResultRepository resultRepo,
        IClassRepository classRepo)
    {
        _assessmentRepo = assessmentRepo;
        _resultRepo = resultRepo;
        _classRepo = classRepo;
    }

    public async Task<Result<AssessmentResponseDto>> CreateAsync(CreateAssessmentDto request)
    {
        var errors = AssessmentGuard.Validate(request);
        if (errors.Any()) return Result<AssessmentResponseDto>.Fail(errors);

        var cls = await _classRepo.GetByIdAsync(request.ClassId);
        if (cls is null) return Result<AssessmentResponseDto>.Fail("Class not found.");

        var sequence = await _assessmentRepo.GetNextSequenceAsync();
        var code = AssessmentCodeGenerator.Generate(sequence);

        var assessment = new Assessment
        {
            Code = code,
            Title = request.Title.Trim(),
            Description = request.Description,
            ClassId = request.ClassId,
            Type = request.Type,
            Status = AssessmentStatus.Draft,
            ScheduledDate = request.ScheduledDate.Date,
            MaxScore = request.MaxScore,
            Weight = request.Weight
        };

        await _assessmentRepo.AddAsync(assessment);
        await _assessmentRepo.SaveChangesAsync();

        return Result<AssessmentResponseDto>.Ok(
            MapToDto(assessment, cls, 0, 0, 0),
            $"Assessment {code} created.");
    }

    public async Task<Result<AssessmentResponseDto>> GetByIdAsync(Guid id)
    {
        var assessment = await _assessmentRepo.GetByIdAsync(id);
        if (assessment is null) return Result<AssessmentResponseDto>.Fail("Assessment not found.");

        var results = await _resultRepo.GetByAssessmentAsync(id);
        var (avgScore, avgPct) = ComputeAverages(results, assessment.MaxScore);

        return Result<AssessmentResponseDto>.Ok(
            MapToDto(assessment, assessment.Class, results.Count, avgScore, avgPct));
    }

    public async Task<Result<List<AssessmentResponseDto>>> GetAllAsync()
    {
        var assessments = await _assessmentRepo.GetAllAsync();
        var result = new List<AssessmentResponseDto>();

        foreach (var a in assessments)
        {
            var results = await _resultRepo.GetByAssessmentAsync(a.Id);
            var (avgScore, avgPct) = ComputeAverages(results, a.MaxScore);
            result.Add(MapToDto(a, a.Class, results.Count, avgScore, avgPct));
        }

        return Result<List<AssessmentResponseDto>>.Ok(result);
    }

    public async Task<Result<List<AssessmentResponseDto>>> GetByClassAsync(Guid classId)
    {
        var assessments = await _assessmentRepo.GetByClassAsync(classId);
        var result = new List<AssessmentResponseDto>();

        foreach (var a in assessments)
        {
            var results = await _resultRepo.GetByAssessmentAsync(a.Id);
            var (avgScore, avgPct) = ComputeAverages(results, a.MaxScore);
            result.Add(MapToDto(a, a.Class, results.Count, avgScore, avgPct));
        }

        return Result<List<AssessmentResponseDto>>.Ok(result);
    }

    public async Task<Result<AssessmentResponseDto>> UpdateAsync(Guid id, UpdateAssessmentDto request)
    {
        var errors = AssessmentGuard.Validate(request);
        if (errors.Any()) return Result<AssessmentResponseDto>.Fail(errors);

        var assessment = await _assessmentRepo.GetByIdAsync(id);
        if (assessment is null) return Result<AssessmentResponseDto>.Fail("Assessment not found.");

        // If max score is being reduced below existing scores, reject
        var existing = await _resultRepo.GetByAssessmentAsync(id);
        if (existing.Any() && existing.Max(r => r.Score) > request.MaxScore)
            return Result<AssessmentResponseDto>.Fail(
                $"Max score cannot be lower than the highest recorded score ({existing.Max(r => r.Score)}).");

        assessment.Title = request.Title.Trim();
        assessment.Description = request.Description;
        assessment.Type = request.Type;
        assessment.Status = request.Status;
        assessment.ScheduledDate = request.ScheduledDate.Date;
        assessment.MaxScore = request.MaxScore;
        assessment.Weight = request.Weight;

        await _assessmentRepo.SaveChangesAsync();

        var (avgScore, avgPct) = ComputeAverages(existing, assessment.MaxScore);
        return Result<AssessmentResponseDto>.Ok(
            MapToDto(assessment, assessment.Class, existing.Count, avgScore, avgPct),
            "Assessment updated.");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var assessment = await _assessmentRepo.GetByIdAsync(id);
        if (assessment is null) return Result<bool>.Fail("Assessment not found.");

        assessment.IsActive = false;
        assessment.Status = AssessmentStatus.Cancelled;
        await _assessmentRepo.SaveChangesAsync();

        return Result<bool>.Ok(true, "Assessment cancelled.");
    }

    public async Task<Result<List<ResultResponseDto>>> GetResultsAsync(Guid assessmentId)
    {
        var assessment = await _assessmentRepo.GetByIdAsync(assessmentId);
        if (assessment is null) return Result<List<ResultResponseDto>>.Fail("Assessment not found.");

        var results = await _resultRepo.GetByAssessmentAsync(assessmentId);

        var dtos = results.Select(r => new ResultResponseDto
        {
            Id = r.Id,
            AssessmentId = r.AssessmentId,
            LearnerId = r.LearnerId,
            LearnerCode = r.Learner?.Code ?? string.Empty,
            LearnerName = r.Learner is null ? "—" : $"{r.Learner.FirstName} {r.Learner.LastName}",
            Score = r.Score,
            MaxScore = assessment.MaxScore,
            Percentage = assessment.MaxScore > 0
                ? Math.Round(r.Score / assessment.MaxScore * 100, 1)
                : 0,
            Notes = r.Notes
        }).ToList();

        return Result<List<ResultResponseDto>>.Ok(dtos);
    }

    public async Task<Result<bool>> SaveResultsAsync(SaveResultsDto request, Guid? recordedByUserId)
    {
        var assessment = await _assessmentRepo.GetByIdAsync(request.AssessmentId);
        if (assessment is null) return Result<bool>.Fail("Assessment not found.");

        if (assessment.Status == AssessmentStatus.Cancelled)
            return Result<bool>.Fail("Cannot record results for a cancelled assessment.");

        if (request.Results is null || request.Results.Count == 0)
            return Result<bool>.Fail("No results to save.");

        // Validate all scores first
        foreach (var item in request.Results.Where(r => r.Score.HasValue))
        {
            var errs = AssessmentGuard.ValidateScore(item.Score!.Value, assessment.MaxScore);
            if (errs.Any())
                return Result<bool>.Fail(
                    $"Invalid score for one or more learners: {string.Join(" ", errs)}");
        }

        // Soft-delete existing results for this assessment
        var existing = await _resultRepo.GetByAssessmentAsync(request.AssessmentId);
        foreach (var old in existing)
            old.IsActive = false;

        // Add new results (skip entries with null score — they weren't graded)
        var newResults = request.Results
            .Where(r => r.Score.HasValue)
            .Select(r => new Result
            {
                AssessmentId = request.AssessmentId,
                LearnerId = r.LearnerId,
                Score = r.Score!.Value,
                Notes = r.Notes,
                RecordedByUserId = recordedByUserId
            })
            .ToList();

        if (newResults.Any())
            await _resultRepo.AddRangeAsync(newResults);

        // If results are recorded, mark the assessment as Completed (unless Draft)
        if (newResults.Any() && assessment.Status == AssessmentStatus.Scheduled)
            assessment.Status = AssessmentStatus.Completed;

        await _resultRepo.SaveChangesAsync();
        await _assessmentRepo.SaveChangesAsync();

        return Result<bool>.Ok(true, $"Results saved for {newResults.Count} learner(s).");
    }

    private static (decimal AvgScore, decimal AvgPercentage) ComputeAverages(
        List<Result> results, decimal maxScore)
    {
        if (!results.Any() || maxScore <= 0) return (0, 0);
        var avg = results.Average(r => r.Score);
        return (Math.Round(avg, 2), Math.Round(avg / maxScore * 100, 1));
    }

    private static AssessmentResponseDto MapToDto(
        Assessment a, Class? cls, int resultsRecorded,
        decimal avgScore, decimal avgPct) => new()
        {
            Id = a.Id,
            Code = a.Code,
            Title = a.Title,
            Description = a.Description,
            ClassId = a.ClassId,
            ClassCode = cls?.Code ?? string.Empty,
            ClassName = cls?.Name ?? string.Empty,
            Type = (int)a.Type,
            TypeName = a.Type.ToString(),
            Status = (int)a.Status,
            StatusName = a.Status.ToString(),
            ScheduledDate = a.ScheduledDate,
            MaxScore = a.MaxScore,
            Weight = a.Weight,
            ResultsRecorded = resultsRecorded,
            AverageScore = avgScore,
            AveragePercentage = avgPct,
            CreatedAt = a.CreatedAt
        };
}