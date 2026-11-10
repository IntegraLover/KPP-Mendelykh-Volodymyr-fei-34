using System;
using System.Collections.Generic;
using System.Linq;
using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class FleetService
{
    private readonly IVehicleStore _store;

    public FleetService(IVehicleStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    // Подія зміни стану
    public event EventHandler<FleetChangedEventArgs>? Changed;

    // Приватний метод для безпечного підняття події після змін
    private void OnChanged(Vehicle v, FleetChangeKind kind, int mileageDelta) =>
        Changed?.Invoke(this, new FleetChangedEventArgs(v.Id, kind, mileageDelta, v.Mileage));

    public Vehicle RegisterVehicle(string vin, string model, string category, decimal dailyRate, int initialMileage)
    {
        var vehicle = Vehicle.Create(
            Guid.NewGuid().ToString("N")[..8],
            vin,
            model,
            category,
            dailyRate,
            initialMileage
        );
        
        _store.Add(vehicle);
        OnChanged(vehicle, FleetChangeKind.Registered, initialMileage);
        return vehicle;
    }

    public void RentVehicle(string id)
    {
        var vehicle = _store.GetById(id)
            ?? throw new InvalidOperationException($"Немає авто з id={id}.");

        vehicle.Rent();
        _store.Update(vehicle);

        OnChanged(vehicle, FleetChangeKind.Rented, 0);
    }

    public void ReturnVehicle(string id, int addedMileage)
    {
        var vehicle = _store.GetById(id)
            ?? throw new InvalidOperationException($"Немає авто з id={id}.");

        vehicle.Return(addedMileage);
        _store.Update(vehicle);

        OnChanged(vehicle, FleetChangeKind.Returned, addedMileage);
    }

    // Метод фільтрації з використанням делегата Func
    public IReadOnlyList<Vehicle> FindAll(Func<Vehicle, bool> predicate) =>
        _store.List().Where(predicate).ToList();
}