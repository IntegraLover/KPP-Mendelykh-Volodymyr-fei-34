using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Services;
using Core.Storage;
using Xunit;

namespace Core.Tests;

public sealed class ImportTests
{
    [Fact]
    // 6) Кооперативне скасування при заздалегідь скасованому токені
    public async Task ImportAsync_WhenAlreadyCancelled_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var service = new FleetService(new InMemoryVehicleStore());

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => service.ImportAsync(new JsonVehicleImporter(), "any.json", cts.Token));
    }

    [Fact]
    // 7) Валідація JSON-даних: некоректний тариф перетворюється на DomainException із номером запису
    public async Task ImportAsync_WithInvalidRecordInJson_ThrowsDomainException()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"autofleet_test_{Guid.NewGuid():N}.json");
        try
        {
            var badJson = """
            [
              { "Id": "v-10", "Vin": "VIN-10", "Model": "Mazda 3", "Category": "Passenger", "DailyRate": 45 },
              { "Id": "v-11", "Vin": "VIN-11", "Model": "Broken Car", "Category": "SUV", "DailyRate": -10 }
            ]
            """;
            await File.WriteAllTextAsync(tempFile, badJson);

            var service = new FleetService(new InMemoryVehicleStore());
            var ex = await Assert.ThrowsAsync<DomainException>(
                () => service.ImportAsync(new JsonVehicleImporter(), tempFile));

            Assert.Contains("Запис 2", ex.Message);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
