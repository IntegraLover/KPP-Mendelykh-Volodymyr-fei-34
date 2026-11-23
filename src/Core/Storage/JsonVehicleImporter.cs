using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class JsonVehicleImporter : IVehicleImporter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IReadOnlyList<Vehicle>> LoadAsync(string path, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        await using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);

        List<VehicleDto>? dtos;
        try
        {
            dtos = await JsonSerializer.DeserializeAsync<List<VehicleDto>>(stream, JsonOptions, ct);
        }
        catch (JsonException ex)
        {
            throw new DomainException($"Помилка формату JSON у файлі {Path.GetFileName(path)}: {ex.Message}", ex);
        }

        if (dtos is null)
            throw new DomainException($"Файл {Path.GetFileName(path)} не містить списку автомобілів.");

        var result = new List<Vehicle>(dtos.Count);
        for (var i = 0; i < dtos.Count; i++)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                result.Add(Vehicle.FromDto(dtos[i]));
            }
            catch (ArgumentException ex)
            {
                throw new DomainException($"Запис {i + 1}: {ex.Message}", ex);
            }
        }

        return result;
    }
}