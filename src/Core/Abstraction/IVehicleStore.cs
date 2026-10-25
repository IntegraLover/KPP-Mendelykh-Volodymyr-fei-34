using System.Collections.Generic;
using Core.Domain;

namespace Core.Abstractions;

public interface IVehicleStore
{
    IReadOnlyList<Vehicle> List();
    Vehicle? GetById(string id);
    void Add(Vehicle item);
    void Update(Vehicle item);
    bool Remove(string id);
}