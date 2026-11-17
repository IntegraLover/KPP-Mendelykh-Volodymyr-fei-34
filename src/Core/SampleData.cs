using System.Collections.Generic;
using Core.Domain;

namespace Core;

public static class SampleData
{
    public static List<Vehicle> CreateFleet() =>
    [
        Vehicle.Create("v-001", "VIN-10001", "Toyota Corolla", "Passenger", 35m, 52000),
        Vehicle.Create("v-002", "VIN-10002", "VW Passat", "Passenger", 45m, 115000),
        Vehicle.Create("v-003", "VIN-10003", "Skoda Octavia", "Passenger", 42m, 185000),
        Vehicle.Create("v-004", "VIN-10004", "Hyundai Elantra", "Passenger", 38m, 41000),
        Vehicle.Create("v-005", "VIN-10005", "Mazda 6", "Passenger", 50m, 95000),

        Vehicle.Create("v-006", "VIN-20001", "Toyota RAV4", "SUV", 65m, 78000),
        Vehicle.Create("v-007", "VIN-20002", "BMW X5", "SUV", 110m, 142000),
        Vehicle.Create("v-008", "VIN-20003", "Audi Q7", "SUV", 115m, 64000),
        Vehicle.Create("v-009", "VIN-20004", "Kia Sportage", "SUV", 58m, 198000),
        Vehicle.Create("v-010", "VIN-20005", "Ford Kuga", "SUV", 55m, 89000),

        Vehicle.Create("v-011", "VIN-30001", "Mercedes Sprinter", "Van", 85m, 260000),
        Vehicle.Create("v-012", "VIN-30002", "VW Transporter", "Van", 75m, 210000),
        Vehicle.Create("v-013", "VIN-30003", "Renault Trafic", "Van", 70m, 155000),
        Vehicle.Create("v-014", "VIN-30004", "Ford Transit", "Van", 72m, 130000),

        Vehicle.Create("v-015", "VIN-40001", "Tesla Model 3", "Electric", 90m, 34000),
        Vehicle.Create("v-016", "VIN-40002", "VW ID.4", "Electric", 80m, 28000),
        Vehicle.Create("v-017", "VIN-40003", "Nissan Leaf", "Electric", 50m, 112000)
    ];
}