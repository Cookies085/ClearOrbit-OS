using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Academy;

namespace ClearOrbit.Application.Interfaces;

public interface IAssessmentService
{
    Task<Result<AssessmentResponseDto>> CreateAsync(CreateAssessmentDto request);
    Task<Result<AssessmentResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<AssessmentResponseDto>>> GetAllAsync();
    Task<Result<List<AssessmentResponseDto>>> GetByClassAsync(Guid classId);
    Task<Result<AssessmentResponseDto>> UpdateAsync(Guid id, UpdateAssessmentDto request);
    Task<Result<bool>> DeleteAsync(Guid id);

    Task<Result<List<ResultResponseDto>>> GetResultsAsync(Guid assessmentId);
    Task<Result<bool>> SaveResultsAsync(SaveResultsDto request, Guid? recordedByUserId);
}