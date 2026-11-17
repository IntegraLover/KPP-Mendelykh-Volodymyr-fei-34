using System;
using System.Collections.Generic;
using System.Linq;
using Core.Domain;

namespace Core.Reports;

public static class ReportService
{
    
    public static IReadOnlyList<VehicleReportLine> HighMileageVehicles(IEnumerable<Vehicle> source, int minMileage) =>
        source.Where(v => v.Mileage >= minMileage)
              .OrderByDescending(v => v.Mileage)
              .ThenBy(v => v.Id, StringComparer.Ordinal)
              .Select(v => ToLine(v))
              .ToList();

    public static IReadOnlyList<VehicleReportLine> TopExpensive(IEnumerable<Vehicle> source, int n) =>
        source.OrderByDescending(v => v.DailyRate)
              .ThenBy(v => v.Id, StringComparer.Ordinal)
              .Take(n)
              .Select(v => ToLine(v))
              .ToList();

    public static IReadOnlyList<CategorySummary> ByCategory(IEnumerable<Vehicle> source) =>
        source.GroupBy(v => v.Category, StringComparer.OrdinalIgnoreCase)
              .Select(g => new CategorySummary(
                  g.Key,
                  g.Count(),
                  g.Sum(v => v.Mileage),
                  g.Average(v => v.DailyRate)))
              .OrderByDescending(s => s.Count)
              .ThenBy(s => s.Category, StringComparer.OrdinalIgnoreCase)
              .ToList();

    public static (int Items, int TotalMileage, double AverageMileage, int MaxMileage) Totals(IEnumerable<Vehicle> source)
    {
        var list = source.ToList();
        return list.Count == 0
            ? (0, 0, 0d, 0)
            : (list.Count,
               list.Sum(v => v.Mileage),
               list.Average(v => v.Mileage),
               list.Max(v => v.Mileage));
    }

    private static VehicleReportLine ToLine(Vehicle v) =>
        new(v.Id, v.Vin, v.Model, v.Category, v.Mileage, v.DailyRate, v.IsRented);
}