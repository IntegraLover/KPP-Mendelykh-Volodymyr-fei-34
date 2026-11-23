using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Reports;
using Core.Services;
using Core.Storage;

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

// Для демонстрації таймауту на захисті можна розкоментувати рядок нижче:
// cts.CancelAfter(TimeSpan.FromSeconds(2));

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "fleet.json");

IVehicleStore store = useFile
    ? new FileVehicleStore(dataPath)
    : new InMemoryVehicleStore(SampleData.CreateFleet());

var service = new FleetService(store);
var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? (useFile ? "import" : "report-top");

try
{
    switch (command)
    {
        case "import":
            // Якщо вказано шлях другим аргументом, беремо його, інакше стандартний dataPath
            var importPath = args.Skip(1).FirstOrDefault(a => !a.StartsWith("--")) ?? dataPath;
            var added = await service.ImportAsync(new JsonVehicleImporter(), importPath, cts.Token);
            Console.WriteLine($"Імпортовано записів: {added}");
            break;

        case "report-top":
            Console.WriteLine("=== ТОП-5 НАЙДОРОЖЧИХ АВТО ===");
            Print(ReportService.TopExpensive(service.FindAll(_ => true), 5));
            break;

        case "report-high-mileage":
            Console.WriteLine("=== АВТО З ПРОБІГОМ ВІД 150 000 КМ ===");
            Print(ReportService.HighMileageVehicles(service.FindAll(_ => true), 150_000));
            break;

        case "report-groups":
            Console.WriteLine("=== ЗВЕДЕННЯ ЗА КАТЕГОРІЯМИ ===");
            foreach (var g in ReportService.ByCategory(service.FindAll(_ => true)))
            {
                Console.WriteLine($"{g.Category,-10} позицій: {g.Count,3}  сумарний пробіг: {g.TotalMileage,8} км  сер. тариф: {g.AverageRate:F2}$");
            }
            break;

        case "report-sum":
            Console.WriteLine("=== ЗАГАЛЬНА СТАТИСТИКА АВТОПАРКУ ===");
            var t = ReportService.Totals(service.FindAll(_ => true));
            Console.WriteLine($"Позицій: {t.Items}, сумарний пробіг: {t.TotalMileage} км, середній: {t.AverageMileage:F2} км, макс: {t.MaxMileage} км");
            break;

        case "demo-deferred":
            RunDeferredDemo(service);
            break;

        default:
            Console.WriteLine("Доступні команди: import [шлях] | report-top | report-high-mileage | report-groups | report-sum | demo-deferred");
            break;
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Імпорт скасовано.");
}
catch (DomainException ex)
{
    Console.WriteLine($"Дані відхилено: {ex.Message}");
}
catch (FileNotFoundException)
{
    Console.WriteLine($"Файл не знайдено: {dataPath}");
}

static void Print(IReadOnlyList<VehicleReportLine> lines)
{
    foreach (var l in lines)
    {
        Console.WriteLine($"{l.Id,-7} {l.Model,-20} [{l.Category,-9}] Пробіг: {l.Mileage,7} км | Тариф: {l.DailyRate,6:F2}$");
    }
}

static void RunDeferredDemo(FleetService service)
{
    Console.WriteLine("=== ДЕМО ВІДКЛАДЕНОГО ВИКОНАННЯ LINQ ===");
    var source = service.FindAll(_ => true).ToList();
    var query = source.Where(v => v.Mileage >= 150_000);

    Console.WriteLine($"Авто з пробігом >= 150 000 до змін джерела: {query.Count()}");
    source.Add(Vehicle.Create("tmp-999", "VIN-TMP99", "Volvo FH16", "Truck", 150m, 450_000));
    Console.WriteLine($"Авто з пробігом >= 150 000 після додавання в джерело: {query.Count()}");

    var snapshot = query.ToList();
    source.Clear();
    Console.WriteLine($"У зафіксованому знімку (ToList) лишилось: {snapshot.Count}");
}