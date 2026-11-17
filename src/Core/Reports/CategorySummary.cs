namespace Core.Reports;

public sealed record CategorySummary(
    string Category,
    int Count,
    int TotalMileage,
    decimal AverageRate
);