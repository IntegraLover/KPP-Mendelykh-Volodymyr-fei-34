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
### Структура рішення (Solution)
- `CrossApp.sln` - файл рішення, що об'єднує проєкти[cite: 1].
- `src/Core/` - бібліотека класів (Class Library), містить бізнес-логіку та логіку збору системної інформації (`EnvironmentInfo`)[cite: 1].
- `src/Cli/` - консольний клієнт, що посилається на `Core` через `ProjectReference`[cite: 1].
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