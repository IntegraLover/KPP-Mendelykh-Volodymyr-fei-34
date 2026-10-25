using System.Collections.Generic;
using Core.Domain;

namespace Core;

public static class SampleData
{
    public static List<Vehicle> Vehicles() => new()
    {
        Vehicle.Create("V-001", "1HGCR2F83HA000001", "Honda Accord", "Sedan", 45.00m, 12000, "Гаражний номер 1"),
        Vehicle.Create("V-002", "JTEBU5JR2F5000002", "Toyota RAV4", "SUV", 60.00m, 25000, "Гаражний номер 2"),
        Vehicle.Create("V-003", "WAUZZZ8V1GA000003", "Audi A4", "Sedan", 75.00m, 8000, "Гаражний номер 3")
    };
}