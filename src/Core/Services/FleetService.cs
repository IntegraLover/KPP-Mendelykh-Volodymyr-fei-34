using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    public event EventHandler<FleetChangedEventArgs>? Changed;

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
            ?? throw new DomainException($"Немає авто з id={id}.");

        vehicle.Rent();
        _store.Update(vehicle);

        OnChanged(vehicle, FleetChangeKind.Rented, 0);
    }

    public void ReturnVehicle(string id, int addedMileage)
    {
        var vehicle = _store.GetById(id)
            ?? throw new DomainException($"Немає авто з id={id}.");

        vehicle.Return(addedMileage);
        _store.Update(vehicle);

        OnChanged(vehicle, FleetChangeKind.Returned, addedMileage);
    }

    public async Task<int> ImportAsync(IVehicleImporter importer, string path, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(importer);
        ct.ThrowIfCancellationRequested();

        var items = await importer.LoadAsync(path, ct);
        var added = 0;

        foreach (var item in items)
        {
            ct.ThrowIfCancellationRequested();
            if (_store.GetById(item.Id) is not null) continue; // ідемпотентність імпорту

            _store.Add(item);
            OnChanged(item, FleetChangeKind.Registered, item.Mileage);
            added++;
        }

        return added;
    }

    public IReadOnlyList<Vehicle> FindAll(Func<Vehicle, bool> predicate) =>
        _store.List().Where(predicate).ToList();
}