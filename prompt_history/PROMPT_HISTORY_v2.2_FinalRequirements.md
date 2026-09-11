// PROMPT v2.2: Завершення фінальних вимог проєкту

# Історія промптів: фіналізація Conference Room Booking API

## Стан: [Активна]
## Версія: v2.2
## Дата: 2026-09-11

## Завершені вимоги

1. Виправлено seed-прив’язки `HallService` для залів A, B і C зі стабільними GUID.
2. Додано EF Core migration `InitialCreate` з таблицями залів, послуг, бронювань, BookingService і HallService.
3. Налаштовано XML-документацію Swagger та Bearer security definition.
4. Заповнено README запуском, endpoint-ами, міграціями, JWT і формулами аналітики.
5. Додано загальну обробку непередбачених помилок з логуванням і HTTP 500.
6. Додано integration test через `WebApplicationFactory` для JWT і захищеного API.
7. Додано базову JWT authentication/authorization з Admin role для mutation/report endpoints.
8. Рядковий статус бронювання замінено на `BookingStatus` enum із string conversion у БД.

## Технічні рішення

- Нова БД застосовує migrations через `DatabaseInitializer`.
- Legacy SQLite-база без `__EFMigrationsHistory` підтримується backward-compatible шляхом `EnsureCreated` і seed synchronization.
- `BookingStatus` зберігається як `Booked` або `Cancelled`.
- Demo token endpoint використовує credentials з конфігурації лише для локальної перевірки.

## Перевірка

- `dotnet build ConferenceRoomBooking.sln --nologo` — успішно;
- `dotnet test ConferenceRoomBooking.Tests/ConferenceRoomBooking.Tests.csproj --nologo --no-restore` — 10/10 тестів успішно;
- integration test перевіряє `401` без JWT, отримання token і `201 Created` із Admin JWT;
- migration створює HallService seed-прив’язки та BookingServices.

## Bugfix v2.2.1: PUT hall services synchronization

Проблема: `PUT /api/halls/{id}` повертав `500 DbUpdateConcurrencyException` після `HallServices.Clear()` і graph update через EF Core.

Виправлення:
- додано `UpdateWithServicesAsync` у `IHallRepository`/`HallRepository`;
- старі `HallService` видаляються явно в транзакції;
- зал зберігається окремо, після чого додаються нові зв’язки;
- `DbUpdateConcurrencyException` повертається як `409 Conflict`;
- збережено atomic update для залу та його послуг.

Перевірка:
- реальний JWT-authenticated PUT повернув `200 OK`;
- тестовий зал оновлено з `2200` до `2500` грн/год;
- місткість оновлено з `60` до `70`;
- три послуги успішно прив’язані;
- `dotnet build` успішний;
- `dotnet test` — 10/10 успішно.

## Фінальний prompt

"Завершити Conference Room Booking API: виправити HallService seed, додати EF Core migrations, XML Swagger, README, global 500 handling, integration tests, JWT authorization і BookingStatus enum. Зберегти Clean Architecture, CQRS, Strategy, Builder і перевірити повну збірку та всі тести."