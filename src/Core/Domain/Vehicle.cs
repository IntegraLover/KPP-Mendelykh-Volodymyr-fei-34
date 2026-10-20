using System;
using Core.Dto;

namespace Core.Domain;

public sealed class Vehicle
{
    private decimal _dailyRate;
    private int _mileage;

    public string Id { get; }
    public string Vin { get; }
    public string Model { get; }
    public string Category { get; }
    public string? Note { get; private set; }

    public decimal DailyRate => _dailyRate;
    public int Mileage => _mileage;
    public bool IsRented { get; private set; }

    // Приватний конструктор — створення тільки через фабрику Create
    private Vehicle(string id, string vin, string model, string category, decimal dailyRate, int mileage, bool isRented, string? note)
    {
        Id = id;
        Vin = vin;
        Model = model;
        Category = category;
        _dailyRate = dailyRate;
        _mileage = mileage;
        IsRented = isRented;
        Note = note;
    }

    // Фабричний метод з перевіркою інваріантів під час створення
    public static Vehicle Create(string id, string vin, string model, string category, decimal dailyRate, int mileage = 0, string? note = null)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор автомобіля обов'язковий", nameof(id));

        if (string.IsNullOrWhiteSpace(vin))
            throw new ArgumentException("VIN-код не може бути порожнім", nameof(vin));

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Модель автомобіля обов'язкова", nameof(model));

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Категорія автомобіля обов'язкова", nameof(category));

        if (dailyRate <= 0)
            throw new ArgumentOutOfRangeException(nameof(dailyRate), dailyRate, "Добова ставка оренди має бути більшою за нуль");

        if (mileage < 0)
            throw new ArgumentOutOfRangeException(nameof(mileage), mileage, "Пробіг не може бути від'ємним");

        return new Vehicle(
            id.Trim(),
            vin.Trim().ToUpperInvariant(),
            model.Trim(),
            category.Trim(),
            dailyRate,
            mileage,
            isRented: false,
            note: string.IsNullOrWhiteSpace(note) ? null : note.Trim()
        );
    }

    // Бізнес-метод 1: Взяти в оренду
    public void Rent()
    {
        if (IsRented)
            throw new InvalidOperationException($"Автомобіль {Model} [{Id}] вже перебуває в оренді");

        IsRented = true;
    }

    // Бізнес-метод 2: Повернути з оренди та оновити пробіг
    public void Return(int addedMileage)
    {
        if (!IsRented)
            throw new InvalidOperationException($"Автомобіль {Model} [{Id}] не можна повернути, бо він не був в оренді");

        if (addedMileage <= 0)
            throw new ArgumentOutOfRangeException(nameof(addedMileage), addedMileage, "Пройдений пробіг має бути більшим за нуль");

        _mileage += addedMileage;
        IsRented = false;
    }

    // Бізнес-метод 3: Оновити тариф
    public void UpdateDailyRate(decimal newRate)
    {
        if (newRate <= 0)
            throw new ArgumentOutOfRangeException(nameof(newRate), newRate, "Нова тарифна ставка має бути більшою за нуль");

        _dailyRate = newRate;
    }

    // Мапінг у DTO тижня 3
    public VehicleDto ToDto() => new(Id, Vin, Model, Category, DailyRate, Note);

    // Відновлення з DTO (проходить ВСІ перевірки фабрики Create)
    public static Vehicle FromDto(VehicleDto dto) =>
        Create(dto.Id, dto.Vin, dto.Model, dto.Category, dto.DailyRate, 0, dto.Note);

    public override string ToString() =>
        $"{Id} [{Vin}] {Model} ({Category}) - {DailyRate} грн/день | Пробіг: {Mileage} км | Статус: {(IsRented ? "В оренді" : "Вільне")}";
}