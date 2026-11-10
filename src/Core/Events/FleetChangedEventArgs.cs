using System;

namespace Core.Events;

public sealed class FleetChangedEventArgs(
    string vehicleId,
    FleetChangeKind kind,
    int mileageDelta,
    int currentMileage) : EventArgs
{
    public string VehicleId { get; } = vehicleId;
    public FleetChangeKind Kind { get; } = kind;
    public int MileageDelta { get; } = mileageDelta;
    public int CurrentMileage { get; } = currentMileage;
    public DateTimeOffset At { get; } = DateTimeOffset.Now;
}