# Conference Room Booking API

REST API для керування конференц-залами, бронюваннями, розрахунком вартості та бізнес-аналітикою.

## Стек і архітектура

- .NET 8 Web API
- Clean Architecture: Domain, Application, Infrastructure, API
- EF Core + SQLite
- CQRS через MediatR
- Strategy Pattern для часових тарифів
- Builder Pattern для бізнес-звітів
- FluentValidation і глобальна обробка помилок
- Swagger/OpenAPI

## Запуск

```powershell
dotnet build ConferenceRoomBooking.sln
dotnet run --project src/ConferenceRoomBooking.API --urls http://localhost:5090
```

Swagger UI доступний за адресою:

```text
http://localhost:5090/swagger
```

## Основні endpoints

```text
GET    /api/halls
GET    /api/halls/{id}
POST   /api/halls
PUT    /api/halls/{id}
DELETE /api/halls/{id}
GET    /api/halls/available?startTime=...&endTime=...&capacity=50
GET    /api/services
POST   /api/pricing/calculate
POST   /api/bookings
GET    /api/reports/business?startDate=...&endDate=...
```

## Приклад бронювання

```json
{
	"hallId": "00000000-0000-0000-0000-000000000000",
	"startTime": "2026-09-20T10:00:00Z",
	"durationHours": 2,
	"serviceIds": []
}
```

Система перевіряє місткість, активність залу, доступність послуг і перетин існуючих бронювань. При конфлікті повертається `409 Conflict`.

## Бізнес-звіт

```text
GET /api/reports/business?startDate=2026-09-01T00:00:00Z&endDate=2026-09-30T23:59:59Z
```

Звіт містить:

- загальну виручку;
- кількість активних бронювань;
- завантаженість кожного активного залу;
- найбільш популярний зал;
- популярність додаткових послуг;
- статистику часових слотів.

Для occupancy використовується робочий час `06:00–23:00`, тобто 17 годин на день:

```text
OccupancyRate = BookedHours / AvailableHours * 100
```

Виручка рахується за `Booking.TotalPrice`, а популярність послуг використовує зафіксовані в бронюванні значення `BookingService.UnitPrice`.

## Початкові дані

- Hall A: 50 місць, 2000 грн/год.
- Hall B: 100 місць, 3500 грн/год.
- Hall C: 30 місць, 1500 грн/год.
- Projector: 500 грн.
- Wi-Fi: 300 грн.
- Sound: 700 грн.

## Перевірка

```powershell
dotnet test ConferenceRoomBooking.sln
```

Тести покривають Strategy розрахунку вартості та Builder бізнес-звіту.