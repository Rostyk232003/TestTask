// PROMPT v2.1: ToT-промпт реалізації звітів та аналітики через Builder + CQRS

# Історія промптів: Reports and Analytics

## Стан: [Активна]
## Версія: v2.1
## Дата: 2026-09-11

## Аналіз UML

Надана UML-діаграма описує класичний Builder Pattern у поєднанні з CQRS:

- `GetBusinessReportQuery` — запит на побудову аналітичного звіту;
- `GetBusinessReportQueryHandler` — Client, який отримує дані та запускає побудову;
- `IReportBuilder` — контракт покрокової побудови звіту;
- `BusinessReportBuilder` — конкретний Builder;
- `ReportDirector` — стандартний сценарій побудови;
- `BusinessReport` — Product, готовий аналітичний звіт.

## Архітектурна поправка

У UML handler має пряме поле `AppDbContext`. У Clean Architecture це створило б залежність Application від Infrastructure. Тому реалізація повинна використовувати `IAnalyticsRepository` у Domain/Application contract, а реалізацію цього інтерфейсу розмістити в Infrastructure.

Application не повинен посилатися на `AppDbContext`.

## ToT-вибір

### V1 — CQRS Queries без Builder
Кожен query одразу формує власний DTO.

Переваги:
- найменше коду;
- проста реалізація.

Недоліки:
- handler змішує отримання даних і побудову звіту;
- складніше додавати JSON/CSV/PDF або інші види звітів;
- слабше відповідає наданій UML.

### V2 — CQRS + Builder + Director
Handler отримує агреговані дані через `IAnalyticsRepository`, Director викликає Builder, а Builder формує `BusinessReport`.

Переваги:
- відповідає UML;
- дотримується SRP;
- зберігає Clean Architecture;
- легко додати нові секції та формати;
- зручно тестувати окремо запити, Builder і фінальний DTO.

Недоліки:
- більше класів;
- Director потрібен лише для стандартного сценарію.

### V3 — Повноцінний configurable report engine
Додатково додати каталог секцій, динамічні фільтри та формати експорту.

Переваги:
- максимальна розширюваність.

Недоліки:
- надмірно складно для поточного MVP;
- збільшує ризик зайвої абстракції.

## Обраний варіант

Обрано V2: `CQRS Query + IAnalyticsRepository + ReportDirector + Builder`.

Поточний scope:
- JSON endpoint;
- один стандартний бізнес-звіт;
- період `StartDate`–`EndDate`;
- робочий час для occupancy: `06:00–23:00`;
- активні бронювання: `Status != Cancelled`.

## Промпт на виконання

Виконуємо: Реалізувати модуль звітів та бізнес-аналітики для Conference Room Booking API на .NET 8 у рамках Clean Architecture, CQRS через MediatR і Builder Pattern відповідно до наданої UML-діаграми.

### 1. Архітектурні обмеження

1. Не змінювати без потреби вже реалізовані Strategy, CQRS для залів і бронювання.
2. Application не повинен залежати від Infrastructure або напряму використовувати `AppDbContext`.
3. Створити контракт `IAnalyticsRepository` в Application або Domain contract layer.
4. Реалізувати `IAnalyticsRepository` в Infrastructure через EF Core.
5. Контролер не повинен містити SQL, LINQ-аналітику або бізнес-формули.
6. Не використовувати `var` у згенерованому коді.
7. Додати XML-документацію до public API, DTO, handlers і builder methods.

### 2. CQRS Query

Створити:

- `GetBusinessReportQuery : IRequest<BusinessReportDto>`;
- `GetBusinessReportQueryHandler`;
- `GetBusinessReportQueryValidator`;
- `ReportsController`.

Query повинен приймати:

- `StartDate: DateTime`;
- `EndDate: DateTime`.

Правила валідації:

- `StartDate < EndDate`;
- період не може бути порожнім;
- рекомендовано обмежити максимальний період одним роком;
- всі DateTime повинні використовувати узгоджений UTC-підхід.

Endpoint:

```http
GET /api/reports/business?startDate=2026-09-01T00:00:00Z&endDate=2026-09-30T23:59:59Z
```

Успішна відповідь: `200 OK`.

### 3. Analytics repository

Створити application contract `IAnalyticsRepository` з методом отримання даних для звіту.

Repository повинен повернути типізовану модель аналітичних даних, наприклад:

- список активних залів;
- активні бронювання у вибраному періоді;
- snapshot послуг із бронювань.

Infrastructure implementation повинна:

- використовувати `AsNoTracking()` для read-only запиту;
- відбирати бронювання тільки в заданому періоді;
- виключати скасовані бронювання;
- завантажувати `BookingServices`;
- не повертати зайві дані.

Правило перетину періодів:

```text
booking.StartsAt < endDate
AND booking.EndsAt > startDate
```

### 4. Product: BusinessReport

Створити `BusinessReportDto` або `BusinessReport` з такими секціями:

- `StartDate`;
- `EndDate`;
- `TotalRevenue: decimal`;
- `TotalBookings: int`;
- `OccupancyRatePercentage: decimal`;
- `MostPopularHallName: string?`;
- `HallOccupancy: List<HallOccupancyDto>`;
- `ServicePopularity: List<ServicePopularityDto>`;
- `WorkingHoursSlotStats: Dictionary<string, int>`.

`HallOccupancyDto` повинен містити:

- HallId;
- HallName;
- BookedHours;
- AvailableHours;
- OccupancyRatePercentage.

`ServicePopularityDto` повинен містити:

- ServiceId;
- ServiceName;
- UsageCount;
- Revenue.

### 5. Builder Pattern

Створити `IReportBuilder` із fluent methods:

- `Reset()`;
- `SetTimePeriod(startDate, endDate)`;
- `CalculateRevenue(bookings)`;
- `CalculateOccupancy(bookings, halls)`;
- `SetMostPopularHall(bookings, halls)`;
- `SetMostPopularServices(bookings)`;
- `Build()`.

Кожен метод, крім `Build`, повинен повертати `IReportBuilder`.

Створити `BusinessReportBuilder`, який:

- зберігає поточний `BusinessReport`;
- не виконує доступ до БД;
- не залежить від EF Core;
- обчислює показники тільки на переданих даних;
- підтримує повторне використання через `Reset()`;
- не залишає state між різними запитами.

### 6. ReportDirector

Створити `ReportDirector` із залежністю `IReportBuilder`.

Додати метод:

```csharp
BusinessReportDto BuildStandardJsonReport(
    DateTime startDate,
    DateTime endDate,
    List<Booking> bookings,
    List<Hall> halls);
```

Director повинен викликати методи Builder у визначеному порядку:

1. `Reset()`;
2. `SetTimePeriod()`;
3. `CalculateRevenue()`;
4. `CalculateOccupancy()`;
5. `SetMostPopularHall()`;
6. `SetMostPopularServices()`;
7. `Build()`.

Director не повинен містити SQL або EF Core.

### 7. Формули аналітики

#### Revenue

Враховувати тільки активні бронювання:

```text
TotalRevenue = Sum(Booking.TotalPrice)
```

#### Total bookings

```text
TotalBookings = Count(active bookings)
```

#### Working hours

Використовувати робочий діапазон:

```text
06:00–23:00
```

тобто 17 доступних годин на день.

#### Booked hours

Для кожного бронювання рахувати фактичний перетин із requested period і робочим часом. Не рахувати години поза робочим діапазоном.

#### Hall occupancy

```text
OccupancyRatePercentage =
BookedHours / AvailableHours * 100
```

Для нульового `AvailableHours` повертати `0`, а не ділити на нуль.

#### Most popular hall

Обрати зал із найбільшою кількістю активних бронювань у періоді. При однакових значеннях використати стабільне сортування за назвою.

#### Service popularity

Використати `BookingService` snapshot:

```text
UsageCount = Count(BookingService)
Revenue = Sum(BookingService.UnitPrice)
```

Сортування:

1. `UsageCount` за спаданням;
2. `ServiceName` за зростанням.

### 8. ReportsController

Створити:

```http
GET /api/reports/business
```

Controller повинен:

- приймати `startDate` і `endDate`;
- створювати `GetBusinessReportQuery`;
- викликати `ISender.Send`;
- повертати `200 OK` або помилку middleware;
- не містити розрахунків.

### 9. Реєстрація DI

Зареєструвати:

- `IAnalyticsRepository` → `AnalyticsRepository`;
- `IReportBuilder` → `BusinessReportBuilder`;
- `ReportDirector`;
- MediatR handler та validator через існуючу assembly registration.

Builder і Director повинні мати lifetime `Scoped` або `Transient`, щоб state одного звіту не потрапляв в інший HTTP-запит.

### 10. Тести

Додати тести для:

1. порожнього періоду;
2. revenue за період;
3. виключення Cancelled бронювань;
4. підрахунку кількості бронювань;
5. occupancy одного залу;
6. occupancy декількох залів;
7. бронювання, яке частково виходить за межі періоду;
8. обмеження робочим часом `06:00–23:00`;
9. most popular hall;
10. popularity послуг за BookingService snapshot;
11. стабільного сортування при однакових показниках;
12. повторного використання Builder після `Reset()`;
13. HTTP `GET /api/reports/business`;
14. HTTP `400` для неправильного періоду.

Тести повинні перевіряти реальні розрахунки, а не тільки факт виклику mock-методів.

### 11. Swagger і README

1. Додати XML-коментарі для нового endpoint і DTO.
2. Увімкнути генерацію XML documentation file.
3. Підключити XML documentation до Swagger.
4. Оновити README:
   - опис звіту;
   - приклад запиту;
   - приклад відповіді;
   - пояснення occupancy;
   - визначення робочого часу;
   - опис формул revenue і service popularity.

### 12. Prompt history

Оновити цей файл після реалізації:

- додати список фактично змінених файлів;
- додати результати `dotnet build`;
- додати результати `dotnet test`;
- додати приклад реального HTTP-запиту;
- додати фінальний підсумковий prompt;
- зберегти статус `[Активна]`.

### Критерії приймання

Фіча завершена, якщо:

1. `dotnet build ConferenceRoomBooking.sln` завершується успішно.
2. `dotnet test ConferenceRoomBooking.sln` завершується успішно.
3. `GET /api/reports/business` повертає повний JSON-звіт.
4. Cancelled бронювання не впливають на показники.
5. Revenue використовує `Booking.TotalPrice`.
6. Service popularity використовує snapshot `BookingService.UnitPrice`.
7. Occupancy обмежений робочим часом `06:00–23:00`.
8. Application не залежить від `AppDbContext`.
9. Builder і Director відповідають UML.
10. README та Swagger містять документацію нового endpoint.

## Фактично реалізовано

- `BusinessReportDto`, `HallOccupancyDto`, `ServicePopularityDto`;
- `IAnalyticsRepository` у Domain contracts та `AnalyticsRepository` в Infrastructure;
- `GetBusinessReportQuery`, handler і FluentValidation validator;
- `IReportBuilder`, `BusinessReportBuilder`, `ReportDirector`;
- `GET /api/reports/business`;
- DI-реєстрація Builder, Director і analytics repository;
- XML-документація для Swagger;
- README з endpoint-ами, формулами та прикладом запиту;
- unit-тести Builder для виручки, occupancy, cancelled bookings і snapshot послуг.

## Перевірка

- `dotnet build ConferenceRoomBooking.sln --nologo` — успішно;
- `dotnet test ConferenceRoomBooking.sln --nologo` — 9/9 тестів успішно;
- реальний HTTP-запит `GET /api/reports/business` повернув:
    - `TotalRevenue = 4900`;
    - `TotalBookings = 1`;
    - Projector: `UsageCount = 1`, `Revenue = 500`;
    - occupancy для періоду з робочим часом `06:00–23:00`.

## Фінальний prompt

"Реалізувати звітність через CQRS Query, аналітичний read repository, Builder Pattern і ReportDirector у Clean Architecture. Формувати revenue, кількість бронювань, occupancy активних залів за робочим часом 06:00–23:00, популярність послуг за BookingService snapshot і стабільний JSON endpoint GET /api/reports/business. Перевірити build, tests і реальний HTTP-запит."
