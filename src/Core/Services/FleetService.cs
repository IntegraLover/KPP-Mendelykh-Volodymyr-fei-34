using System;
using System.Collections.Generic;
using Core.Abstractions;
using Core.Domain;

namespace Core.Services;
public sealed class FleetService
{
    private readonly IVehicleStore _store;

    public FleetService(IVehicleStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }
    public Vehicle RegisterVehicle(string vin, string model, string category, decimal dailyRate, int mileage, string? note = null)
    {
        var id = "V-" + Guid.NewGuid().ToString("N")[..5].ToUpper();
        var vehicle = Vehicle.Create(id, vin, model, category, dailyRate, mileage, note);
        _store.Add(vehicle);
        return vehicle;
    }
    public void RentVehicle(string id)
    {
        var vehicle = _store.GetById(id) 
            ?? throw new InvalidOperationException($"Немає авто з id={id}.");

        vehicle.Rent();
        _store.Update(vehicle);
    }
    public void ReturnVehicle(string id, int addedMileage)
    {
        var vehicle = _store.GetById(id) 
            ?? throw new InvalidOperationException($"Немає авто з id={id}.");

        vehicle.Return(addedMileage);
        _store.Update(vehicle);
    }
    public IReadOnlyList<Vehicle> All() => _store.List();
    public Vehicle? Find(string id) => _store.GetById(id);
}