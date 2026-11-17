namespace Core.Reports;

public sealed record VehicleReportLine(
    string Id,
    string Vin,
    string Model,
    string Category,
    int Mileage,
    decimal DailyRate,
    bool IsRented
);