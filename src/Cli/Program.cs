using System;
using System.IO;
using System.Linq;
using Core;
using Core.Abstractions;
using Core.Services;
using Core.Storage;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "fleet.json");

// Composition Root: Ручний DI
IVehicleStore store = useFile
    ? new FileVehicleStore(dataPath)
    : new InMemoryVehicleStore(SampleData.Vehicles());

var service = new FleetService(store);

Console.WriteLine($" Сховище: {store.GetType().Name} ");

try
{
    // 1. Показати список
    Console.WriteLine("\n Поточний автопарк");
    foreach (var v in service.All())
        Console.WriteLine($" {v.Id} | {v.Model,-15} | Тариф: {v.DailyRate,6:C} | Пробіг: {v.Mileage,6} км | Статус: {(v.IsRented ? "В оренді" : "Вільне")}");

    // 2. Додати новий автомобіль
    Console.WriteLine("\n Реєстрація нового авто");
    var newCar = service.RegisterVehicle("3FA6P0H77HR000004", "Ford Fusion", "Sedan", 50.00m, 15000, "Нове поповнення");
    Console.WriteLine($"Створено: {newCar.Id} - {newCar.Model}");

    // 3. Бізнес-операція (Здати в оренду)
    Console.WriteLine($"\n Оренда авто {newCar.Id}");
    service.RentVehicle(newCar.Id);
    Console.WriteLine($"Авто {newCar.Id} успішно здано в оренду.");

    // 4. Сценарій відмови (Спроба повторної оренди або неіснуючий ID)
    Console.WriteLine("\n Тест сценарію відмови");
    service.RentVehicle(newCar.Id); // Має викинути виняток, бо вже в оренді
}
catch (Exception ex)
{
    Console.WriteLine($"\n[ОБРОБЛЕНО ВИНОК]: {ex.GetType().Name} -> {ex.Message}");
}