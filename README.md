# AutoFleet: Vehicle Rental & Service

## Призначення застосунку
Застосунок призначений для ведення бази автомобілів автопарку, фіксації поточного пробігу, реєстрації договорів короткострокової оренди та планування сервісного ТО.

## Основні сутності
1. Vehicle (Автомобіль) - VIN-код, марка, модель, рік випуску, поточний пробіг, добовий тариф, статус доступності.
2. RentalContract (Договір оренди) - посилання на авто та клієнта, дата початку, планова дата повернення, підсумкова вартість, статус.
3. Client (Клієнт) - ПІБ, номер посвідчення водія, номер телефону, email.
4. ServiceRecord (Запис ТО)  посилання на авто, опис робіт, дата обслуговування, пробіг на момент ремонту, вартість робіт.

## Запуск

```bash
dotnet build
dotnet run --project src/Cli

## Лабораторна робота 2
## Структура рішення (Solution)
- `CrossApp.sln` - файл рішення, що об'єднує проєкти.
- `src/Core/` - бібліотека класів (Class Library), містить бізнес-логіку та логіку збору системної інформації (`EnvironmentInfo`).
- `src/Cli/` - консольний клієнт, що посилається на `Core` через `ProjectReference`.
### Порівняння режимів публікації

win-x64 - Self-contained - 70.5Mb - Runtime не потрібний  
win-x64 - Framework-dependent - 0.17Mb - Runtime потрібний .NET 8.0

### Команди збірки, запуску та публікації
```bash
# Збірка всього рішення
dotnet build

# Запуск консольного застосунку
dotnet run --project src/Cli

# Публікація Self-contained (з вбудованим Runtime)
dotnet publish src/Cli -c Release -r win-x64 --self-contained true -o ./publish-self

# Публікація Framework-dependent (лише код програми)
dotnet publish src/Cli -c Release -r win-x64 --self-contained false -o ./publish-fx

# Прямий запуск зібраного файлу
.\publish-self\Cli.exe


### Лабораторна робота №3. Базові типи домену: records, pattern matching, імпорт CSV

### Структура проєкту
 `src/Core/Dto/VehicleDto.cs` — `record`-тип для представлення автомобіля (`Id`, `Vin`, `Model`, `Category`, `DailyRate`, `Note`).
 `src/Core/Dto/ImportResult.cs` — узагальнений `record`-тип для повернення результатів імпорту (список успішних об'єктів та список помилок).
 `src/Core/Import/VehicleCsvImporter.cs` — статичний клас-імпортер з розбором рядка через `switch expression` та Pattern Matching.
 `data/sample.csv` — тестовий файл із 13 рядків (10 коректних записів i 3 з помилками).

### Формат файлу даних (`data/sample.csv`)
Роздільник: крапка з комою `;` 
Заголовок: присутній (`id;vin;model;category;dailyRate;note`), автоматично пропускається під час розбору.
Десятковий роздільник: крапка `.` (парсинг здійснюється через `CultureInfo.InvariantCulture`).

### Використані патерни Pattern Matching
1. Property Pattern: `{ Length: < 5 }` — перевірка на недостатню кількість колонок.
2. List Patterns & Or Pattern: `["", _, _, _, _, ..] or [_, "", _, _, _, ..]` — виявлення порожніх обов'язкових полів (ID або VIN).
3. Guarded Pattern (`when`): перевірка числового значення `dailyRate` через `decimal.TryParse` з `CultureInfo.InvariantCulture` та перевіркою `rate <= 0`.
4. Positional / List Pattern: `[var id, var vin, var model, var category, var rateStr, var note]` — розбір повноцінного рядка з optional приміткою.

### Запуск та перевірка
Запуск з файлом за замовчуванням (`data/sample.csv`)

dotnet run --project src/Cli

## Лабораторна робота №4: Доменна модель AutoFleet

### Таблиця інваріантів доменної сутності `Vehicle`

| № | Правило (Інваріант) | Тип винятку | Метод перевірки |
|---|---|---|---|
| 1 | Ідентифікатори `Id`, `Vin`, `Model`, `Category` не можуть бути порожніми або містити лише пробіли | `ArgumentException` | `Vehicle.Create` |
| 2 | Добова ставка оренди `DailyRate` має бути строго більшою за нуль (> 0) | `ArgumentOutOfRangeException` | `Vehicle.Create`, `UpdateDailyRate` |
| 3 | Початковий та доданий пробіг `Mileage` не можуть бути від'ємними (>= 0) | `ArgumentOutOfRangeException` | `Vehicle.Create`, `Return` |
| 4 | Повторна видача в оренду вже орендованого автомобіля заборонена | `InvalidOperationException` | `Vehicle.Rent` |
| 5 | Повернення з оренди автомобіля зі статусом "Вільне" заборонене | `InvalidOperationException` | `Vehicle.Return` |