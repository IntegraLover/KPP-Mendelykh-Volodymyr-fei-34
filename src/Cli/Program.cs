using System;
using System.Text;
using Core.Domain;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("=== Сценарій 1: Успішна робота з доменною моделлю ===");

Vehicle car = Vehicle.Create("V-001", "1hgcr2f83ha000001", "Honda Accord", "Sedan", 45.00m, 12000, "Гаражний номер 12");
Console.WriteLine($"Створено: {car}");

car.Rent();
Console.WriteLine($"Після здачі в оренду: {car}");

car.Return(350);
Console.WriteLine($"Після повернення з рейсу: {car}");

Console.WriteLine();
Console.WriteLine("=== Сценарій 2: Порушення інваріантів (захист моделі) ===");

TryDo("Спроба створити авто з від'ємним тарифом", () =>
    Vehicle.Create("V-002", "VIN123456789", "BMW X5", "SUV", -50.00m));

TryDo("Спроба створити авто з порожнім VIN", () =>
    Vehicle.Create("V-003", "", "Audi A6", "Sedan", 100.00m));

TryDo("Спроба повторно здати в оренду орендоване авто", () =>
{
    Vehicle rentedCar = Vehicle.Create("V-004", "VIN999999999", "Toyota Camry", "Sedan", 60.00m);
    rentedCar.Rent();
    rentedCar.Rent(); // Має кинути InvalidOperationException
});

TryDo("Спроба повернути авто, яке не було в оренді", () =>
{
    Vehicle freeCar = Vehicle.Create("V-005", "VIN888888888", "Skoda Octavia", "Sedan", 40.00m);
    freeCar.Return(100); // Має кинути InvalidOperationException
});

TryDo("Спроба передати від'ємний пробіг при поверненні", () =>
{
    Vehicle activeCar = Vehicle.Create("V-006", "VIN777777777", "Ford Fiesta", "Hatchback", 30.00m);
    activeCar.Rent();
    activeCar.Return(-50); // Має кинути ArgumentOutOfRangeException
});

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"{title}: ВИНОК НЕ СПРАЦЮВАВ! Інваріант відсутній.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{title}: [{ex.GetType().Name}] -> {ex.Message}");
    }
}