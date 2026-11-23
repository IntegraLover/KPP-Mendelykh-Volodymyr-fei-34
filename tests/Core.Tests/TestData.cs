using System.Collections.Generic;
using Core.Domain;

namespace Core.Tests;

public static class TestData
{
    public static List<Vehicle> Items() =>
    [
        Vehicle.Create("v-1", "VIN-001", "Toyota Corolla", "Passenger", 40m, 50_000),
        Vehicle.Create("v-2", "VIN-002", "BMW X5", "SUV", 120m, 160_000),
        Vehicle.Create("v-3", "VIN-003", "Audi Q7", "SUV", 110m, 210_000),
        Vehicle.Create("v-4", "VIN-004", "Renault Kangoo", "Van", 55m, 180_000)
    ];
}