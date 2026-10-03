namespace Core.Dto;

public record VehicleDto(
    string Id,
    string Vin,
    string Model,
    string Category,
    decimal DailyRate,
    string? Note = null);