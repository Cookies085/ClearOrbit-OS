using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Academy;

namespace ClearOrbit.Application.Interfaces;

public interface IAttendanceService
{
    Task<Result<List<AttendanceRecordDto>>> GetByClassAndDateAsync(Guid classId, DateTime date);
    Task<Result<List<AttendanceRecordDto>>> GetByClassAsync(Guid classId);
    Task<Result<List<SessionAttendanceDto>>> GetSessionSummaryAsync(Guid classId);
    Task<Result<bool>> SaveAsync(SaveAttendanceDto request, Guid? recordedByUserId);
}