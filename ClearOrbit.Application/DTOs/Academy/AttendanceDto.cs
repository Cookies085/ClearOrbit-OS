using ClearOrbit.Domain.Enums;

namespace ClearOrbit.Application.DTOs.Academy;

public class AttendanceRecordDto
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }
    public Guid LearnerId { get; set; }
    public string LearnerCode { get; set; } = string.Empty;
    public string LearnerName { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class SessionAttendanceDto
{
    public DateTime SessionDate { get; set; }
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalLate { get; set; }
    public int TotalExcused { get; set; }
    public int TotalRecords { get; set; }
    public decimal AttendanceRate { get; set; }
}

public class SaveAttendanceItemDto
{
    public Guid LearnerId { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class SaveAttendanceDto
{
    public Guid ClassId { get; set; }
    public DateTime SessionDate { get; set; }
    public List<SaveAttendanceItemDto> Records { get; set; } = new();
}