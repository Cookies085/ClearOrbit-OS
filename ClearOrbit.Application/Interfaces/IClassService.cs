using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Academy;

namespace ClearOrbit.Application.Interfaces;

public interface IClassService
{
    Task<Result<ClassResponseDto>> CreateAsync(CreateClassDto request);
    Task<Result<ClassResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<ClassResponseDto>>> GetAllAsync();
    Task<Result<ClassResponseDto>> UpdateAsync(Guid id, UpdateClassDto request);
    Task<Result<bool>> DeleteAsync(Guid id);

    Task<Result<List<EnrollmentResponseDto>>> GetEnrollmentsAsync(Guid classId);
    Task<Result<EnrollmentResponseDto>> EnrollLearnerAsync(CreateEnrollmentDto request);
    Task<Result<bool>> WithdrawLearnerAsync(Guid enrollmentId);
}