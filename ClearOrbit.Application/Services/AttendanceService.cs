using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Academy;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendanceRepo;
    private readonly IClassRepository _classRepo;
    private readonly IEnrollmentRepository _enrollmentRepo;

    public AttendanceService(
        IAttendanceRepository attendanceRepo,
        IClassRepository classRepo,
        IEnrollmentRepository enrollmentRepo)
    {
        _attendanceRepo = attendanceRepo;
        _classRepo = classRepo;
        _enrollmentRepo = enrollmentRepo;
    }

    public async Task<Result<List<AttendanceRecordDto>>> GetByClassAndDateAsync(Guid classId, DateTime date)
    {
        var records = await _attendanceRepo.GetByClassAndDateAsync(classId, date.Date);
        return Result<List<AttendanceRecordDto>>.Ok(records.Select(MapToDto).ToList());
    }

    public async Task<Result<List<AttendanceRecordDto>>> GetByClassAsync(Guid classId)
    {
        var records = await _attendanceRepo.GetByClassAsync(classId);
        return Result<List<AttendanceRecordDto>>.Ok(records.Select(MapToDto).ToList());
    }

    public async Task<Result<List<SessionAttendanceDto>>> GetSessionSummaryAsync(Guid classId)
    {
        var records = await _attendanceRepo.GetByClassAsync(classId);

        var sessions = records
            .GroupBy(r => r.SessionDate.Date)
            .Select(g =>
            {
                var present = g.Count(r => r.Status == AttendanceStatus.Present);
                var late = g.Count(r => r.Status == AttendanceStatus.Late);
                var absent = g.Count(r => r.Status == AttendanceStatus.Absent);
                var excused = g.Count(r => r.Status == AttendanceStatus.Excused);
                var total = g.Count();
                var attended = present + late;

                return new SessionAttendanceDto
                {
                    SessionDate = g.Key,
                    TotalPresent = present,
                    TotalAbsent = absent,
                    TotalLate = late,
                    TotalExcused = excused,
                    TotalRecords = total,
                    AttendanceRate = total == 0 ? 0 : Math.Round((decimal)attended / total * 100, 1)
                };
            })
            .OrderByDescending(s => s.SessionDate)
            .ToList();

        return Result<List<SessionAttendanceDto>>.Ok(sessions);
    }

    public async Task<Result<bool>> SaveAsync(SaveAttendanceDto request, Guid? recordedByUserId)
    {
        var cls = await _classRepo.GetByIdAsync(request.ClassId);
        if (cls is null) return Result<bool>.Fail("Class not found.");

        if (request.Records is null || request.Records.Count == 0)
            return Result<bool>.Fail("No attendance records to save.");

        var sessionDate = request.SessionDate.Date;

        // Remove any existing records for this class+date (idempotent save)
        var existing = await _attendanceRepo.GetByClassAndDateAsync(request.ClassId, sessionDate);
        foreach (var old in existing)
            old.IsActive = false;

        var newRecords = request.Records.Select(r => new AttendanceRecord
        {
            ClassId = request.ClassId,
            LearnerId = r.LearnerId,
            SessionDate = sessionDate,
            Status = r.Status,
            Notes = r.Notes,
            RecordedByUserId = recordedByUserId
        }).ToList();

        await _attendanceRepo.AddRangeAsync(newRecords);
        await _attendanceRepo.SaveChangesAsync();

        return Result<bool>.Ok(true, $"Attendance saved for {sessionDate:yyyy-MM-dd}.");
    }

    private static AttendanceRecordDto MapToDto(AttendanceRecord a) => new()
    {
        Id = a.Id,
        ClassId = a.ClassId,
        LearnerId = a.LearnerId,
        LearnerCode = a.Learner?.Code ?? string.Empty,
        LearnerName = a.Learner is null ? "—" : $"{a.Learner.FirstName} {a.Learner.LastName}",
        SessionDate = a.SessionDate,
        Status = (int)a.Status,
        StatusName = a.Status.ToString(),
        Notes = a.Notes
    };
}