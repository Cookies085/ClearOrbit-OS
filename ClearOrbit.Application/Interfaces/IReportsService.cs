using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Reports;

namespace ClearOrbit.Application.Interfaces;

public interface IReportsService
{
    Task<Result<ReportsSummaryDto>> GetSummaryAsync();
    Task<Result<List<DivisionProfitabilityDto>>> GetDivisionProfitabilityAsync();
    Task<Result<List<ClientProfitabilityDto>>> GetClientProfitabilityAsync();
    Task<Result<List<ProjectProfitabilityDto>>> GetProjectProfitabilityAsync();
}