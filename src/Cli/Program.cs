using Core.Dto;
using Core.Import;

string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

ImportResult<VehicleDto> result = VehicleCsvImporter.Load(path);

Console.WriteLine($"Завантажено записів: {result.Items.Count}");
Console.WriteLine(new string('-', 65));

foreach (VehicleDto v in result.Items.Take(5))
{
    Console.WriteLine($"{v.Id,-7} {v.Vin,-20} {v.Model,-18} {v.DailyRate,7:C2}  {v.Note}");
}

if (result.Errors.Count > 0)
{
    Console.WriteLine(new string('-', 65));
    Console.WriteLine($"Пропущено рядків: {result.Errors.Count}");
    foreach (string e in result.Errors)
    {
        Console.WriteLine($" ! {e}");
    }
}

return 0;