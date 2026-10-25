using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class FileVehicleStore : IVehicleStore
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
    private readonly Dictionary<string, Vehicle> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _path;
    private bool _loaded;

    public FileVehicleStore(string path)
    {
        _path = Path.GetFullPath(path);
    }

    private void EnsureLoaded()
    {
        if (_loaded) return;

        if (File.Exists(_path))
        {
            var json = File.ReadAllText(_path);
            var dtos = JsonSerializer.Deserialize<List<VehicleDto>>(json) ?? [];

            foreach (var dto in dtos)
            {
                var v = Vehicle.FromDto(dto);
                _cache[v.Id] = v;
            }
        }

        _loaded = true;
    }

    private void Flush()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var dtos = _cache.Values.Select(v => v.ToDto()).ToList();
        File.WriteAllText(_path, JsonSerializer.Serialize(dtos, Options));
    }

    public IReadOnlyList<Vehicle> List()
    {
        EnsureLoaded();
        return _cache.Values.ToList();
    }

    public Vehicle? GetById(string id)
    {
        EnsureLoaded();
        return _cache.GetValueOrDefault(id);
    }

    public void Add(Vehicle item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureLoaded();

        if (_cache.ContainsKey(item.Id))
            throw new InvalidOperationException($"Запис з id={item.Id} уже існує.");

        _cache.Add(item.Id, item);
        Flush();
    }

    public void Update(Vehicle item)
    {
        EnsureLoaded();
        _cache[item.Id] = item;
        Flush();
    }

    public bool Remove(string id)
    {
        EnsureLoaded();
        if (!_cache.Remove(id)) return false;
        Flush();
        return true;
    }
}