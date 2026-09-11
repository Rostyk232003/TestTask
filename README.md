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

## Database migrations

Нова база автоматично застосовує EF Core migrations під час запуску. Для ручного керування схемою:

```powershell
dotnet ef database update --project src/ConferenceRoomBooking.Infrastructure/ConferenceRoomBooking.Infrastructure.csproj --startup-project src/ConferenceRoomBooking.API/ConferenceRoomBooking.API.csproj
```

Початкова міграція зберігається у `src/ConferenceRoomBooking.Infrastructure/Persistence/Migrations`.

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
POST   /api/auth/token
```

## JWT authentication

Операції створення, редагування та видалення залів, бронювання і бізнес-звіт захищені JWT. Для локальної перевірки отримайте demo admin token:

```json
POST /api/auth/token
{
  "username": "admin",
  "password": "ChangeThisDevelopmentPassword!"
}
```

Передавайте отриманий токен у заголовку:

```text
Authorization: Bearer <accessToken>
```

Demo credentials призначені лише для Development. У production потрібно використовувати зовнішній Identity Provider або секрети з захищеного сховища.

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

Статус бронювання представлений enum `BookingStatus` (`Booked`, `Cancelled`) і зберігається в БД як читабельне значення.

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

Стандартні seed UUID послуг:

- Projector: `b77393a7-a368-49fb-ae51-b43a3c4a4bca`;
- Wi-Fi: `0d807f28-2976-4c81-b182-5db457c51819`;
- Sound: `9492a6e0-8ee7-419c-a244-a02e379c2bcd`.

## Перевірка

```powershell
dotnet test ConferenceRoomBooking.sln
```

Тести покривають Strategy розрахунку вартості та Builder бізнес-звіту.

Інтеграційні тести через `WebApplicationFactory` перевіряють JWT authorization, token endpoint і захищене створення залу на ізольованій SQLite-базі.

Повна покрокова інструкція ручної перевірки Swagger UI: [docs/SWAGGER_TESTING.md](docs/SWAGGER_TESTING.md).
