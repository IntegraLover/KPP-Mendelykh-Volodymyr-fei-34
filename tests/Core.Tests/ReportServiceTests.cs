using System.Linq;
using Core.Reports;
using Xunit;

namespace Core.Tests;

public sealed class ReportServiceTests
{
    [Theory]
    [InlineData(150_000, 3)]
    [InlineData(200_000, 1)]
    [InlineData(300_000, 0)]
    // 4) Фільтрація за порогом пробігу на кількох наборах вхідних даних
    public void HighMileageVehicles_CountsOnlyAboveThreshold(int minMileage, int expectedCount)
    {
        var result = ReportService.HighMileageVehicles(TestData.Items(), minMileage);
        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    // 5) Детермінований топ-N за тарифом
    public void TopExpensive_IsDeterministic()
    {
        var top = ReportService.TopExpensive(TestData.Items(), 2);
        Assert.Equal(new[] { "v-2", "v-3" }, top.Select(x => x.Id));
    }
}