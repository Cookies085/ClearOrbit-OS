using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Dashboard;

namespace ClearOrbit.Application.Interfaces;

public interface IDashboardService
{
    Task<Result<DashboardSummaryDto>> GetSummaryAsync();
}