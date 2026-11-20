namespace StorePilot.Application.Dtos.Dashboard;

public sealed record DashboardSummaryDto(
    int ActiveProductsCount,
    int ActiveGroupsCount,
    int ActiveUnitsCount);
