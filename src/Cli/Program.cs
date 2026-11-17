using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Reports;
using Core.Services;
using Core.Storage;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "fleet.json");

IVehicleStore store = useFile
    ? new FileVehicleStore(dataPath)
    : new InMemoryVehicleStore(SampleData.CreateFleet());

var service = new FleetService(store);
var items = service.FindAll(_ => true); // Отримуємо всі авто зі сховища

// Вибір підкоманди звіту (ігноруємо прапорці типу --file)
var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "report-top";

switch (command)
{
    case "report-top":
        Console.WriteLine("ТОП-5 НАЙДОРОЖЧИХ АВТО");
        Print(ReportService.TopExpensive(items, 5));
        break;

    case "report-high-mileage":
        Console.WriteLine("АВТО З ПРОБІГОМ ВІД 150 000 КМ");
        Print(ReportService.HighMileageVehicles(items, 150_000));
        break;

    case "report-groups":
        Console.WriteLine("ЗВЕДЕННЯ ЗА КАТЕГОРІЯМИ");
        foreach (var g in ReportService.ByCategory(items))
        {
            Console.WriteLine($"{g.Category,-10} позицій: {g.Count,3}  сумарний пробіг: {g.TotalMileage,8} км  сер. тариф: {g.AverageRate:F2}$");
        }
        break;

    case "report-sum":
        Console.WriteLine("ЗАГАЛЬНА СТАТИСТИКА АВТОПАРКУ");
        var t = ReportService.Totals(items);
        Console.WriteLine($"Позицій: {t.Items}, сумарний пробіг: {t.TotalMileage} км, середній: {t.AverageMileage:F2} км, макс: {t.MaxMileage} км");
        break;

    case "demo-deferred":
        RunDeferredDemo(service);
        break;

    default:
        Console.WriteLine("Доступні команди: report-top | report-high-mileage | report-groups | report-sum | demo-deferred");
        break;
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
    Console.WriteLine("ДЕМО ВІДКЛАДЕНОГО ВИКОНАННЯ LINQ ");
    var source = service.FindAll(_ => true).ToList();

    // Запит побудовано, але ще НЕ виконано
    var query = source.Where(v => v.Mileage >= 150_000);

    Console.WriteLine($"Авто з пробігом >= 150 000 до змін джерела: {query.Count()}");

    // Змінюємо джерело після оголошення запиту
    source.Add(Vehicle.Create("tmp-999", "VIN-TMP99", "Volvo FH16", "Truck", 150m, 450_000));

    // Повторний виклик Count() знову перебирає джерело і бачить новий елемент
    Console.WriteLine($"Авто з пробігом >= 150 000 після додавання в джерело: {query.Count()}");

    // Матеріалізуємо результат у список
    var snapshot = query.ToList();
    source.Clear();

    Console.WriteLine($"У зафіксованому знімку (ToList) лишилось: {snapshot.Count}");
}