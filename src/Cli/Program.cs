using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Abstractions;
using Core.Domain;
using Core.Events;
using Core.Services;
using Core.Storage;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "fleet.json");

// Отримуємо початкові дані (або передаємо порожній список, якщо SampleData не використовується)
var initialVehicles = new List<Vehicle>
{
    Vehicle.Create("v-001", "VIN111222333", "Toyota Corolla", "Passenger", 35.0m, 50000),
    Vehicle.Create("v-002", "VIN444555666", "VW Passat", "Passenger", 45.0m, 80000)
};

IVehicleStore store = useFile
    ? new FileVehicleStore(dataPath)
    : new InMemoryVehicleStore(initialVehicles);

var service = new FleetService(store);

// Шлях до лог-файлу
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

// 1. Перший підписник — локальний метод логування
void OnFleetChanged(object? sender, FleetChangedEventArgs e)
{
    var line = $"{e.At:HH:mm:ss} {e.Kind,-10} {e.VehicleId} {e.MileageDelta,4:+0;-0} пробіг {e.CurrentMileage} км";
    Console.WriteLine(line);
    File.AppendAllText(logPath, line + Environment.NewLine);
}

// Підписка
service.Changed += OnFleetChanged;

// 2. Другий підписник — лямбда із замиканням над лічильником
int changes = 0;
service.Changed += (sender, e) => changes++;

Console.WriteLine("Робота з подіями");
var created = service.RegisterVehicle("VIN-999888777", "Skoda Octavia", "Passenger", 45.0m, 120000);
service.RentVehicle(created.Id);
service.ReturnVehicle(created.Id, 150);

Console.WriteLine($"Подій отримано: {changes}");

// Відписка першого підписника (логера)
service.Changed -= OnFleetChanged;

Console.WriteLine("\n Після відписки логера ");
service.RentVehicle(created.Id);

Console.WriteLine($"Подій отримано: {changes}");

// Сценарій відмови
Console.WriteLine("\nСценарій відмови");
try
{
    service.RentVehicle(created.Id);
}
catch (Exception ex)
{
    Console.WriteLine($"[ОБРОБЛЕНО ВИНОК]: {ex.GetType().Name} -> {ex.Message}");
}

Console.WriteLine($"Подій після помилки: {changes}");

// Пошук через Func
Console.WriteLine("\nПошук через Func");
var highMileageVehicles = service.FindAll(v => v.Mileage > 100000);
Console.WriteLine($"Авто з пробігом понад 100 000 км: {highMileageVehicles.Count}");
foreach (var v in highMileageVehicles)
{
    Console.WriteLine($" - {v.Model} (ID: {v.Id}, Пробіг: {v.Mileage} км)");
}