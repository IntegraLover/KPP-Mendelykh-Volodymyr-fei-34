using System;
using System.Collections.Generic;
using Core;
using Core.Domain;
using Core.Events;
using Core.Services;
using Core.Storage;
using Xunit;

namespace Core.Tests;

public sealed class FleetServiceTests
{
    [Fact]
    // 1) Інваріант та доменний виняток: повторна оренда заборонена, стан не змінюється
    public void RentVehicle_WhenAlreadyRented_ThrowsDomainException_AndKeepsState()
    {
        var store = new InMemoryVehicleStore();
        var service = new FleetService(store);
        var v = service.RegisterVehicle("VIN-111", "Skoda Octavia", "Passenger", 45m, 10_000);
        service.RentVehicle(v.Id);

        Assert.Throws<DomainException>(() => service.RentVehicle(v.Id));
        Assert.True(store.GetById(v.Id)!.IsRented);
    }

    [Fact]
    // 2) Перевірка аргументів: некоректний пробіг при поверненні кидає ArgumentOutOfRangeException
    public void ReturnVehicle_WithNonPositiveMileage_ThrowsArgumentOutOfRange()
    {
        var store = new InMemoryVehicleStore();
        var service = new FleetService(store);
        var v = service.RegisterVehicle("VIN-222", "VW Golf", "Passenger", 40m, 20_000);
        service.RentVehicle(v.Id);

        Assert.Throws<ArgumentOutOfRangeException>(() => service.ReturnVehicle(v.Id, 0));
        Assert.Equal(20_000, store.GetById(v.Id)!.Mileage);
    }

    [Fact]
    // 3) Подія Changed піднімається рівно один раз при реєстрації авто
    public void RegisterVehicle_RaisesChangedExactlyOnce()
    {
        var service = new FleetService(new InMemoryVehicleStore());
        var events = new List<FleetChangedEventArgs>();
        service.Changed += (_, e) => events.Add(e);

        service.RegisterVehicle("VIN-333", "Ford Focus", "Passenger", 38m, 15_000);

        Assert.Single(events);
        Assert.Equal(FleetChangeKind.Registered, events[0].Kind);
    }
}