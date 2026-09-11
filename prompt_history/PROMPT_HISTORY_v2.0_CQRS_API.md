// PROMPT v2.0: Реалізація CQRS API для залів і бронювань

# Історія промптів: CQRS API

## Стан: [Активна]

## Версія: v2.0

## Дата: 2026-09-11

## Обраний підхід

Реалізовано CQRS через MediatR відповідно до UML:

- Commands для створення, оновлення, видалення залу та бронювання;
- Query для пошуку доступних залів;
- окремий handler для кожної операції;
- контролери передають запити через `ISender`;
- бізнес-логіка залишається в Application layer.

## Реалізовані сценарії

- `POST /api/halls` — створення залу з послугами;
- `PUT /api/halls/{id}` — оновлення залу та синхронізація послуг;
- `DELETE /api/halls/{id}` — soft delete залу без майбутніх бронювань;
- `GET /api/halls/available` — фільтрація за місткістю та перетином часу;
- `POST /api/bookings` — бронювання із snapshot послуг та Strategy-розрахунком ціни.

## Додаткові рішення

- `BookingService` зберігає назву та ціну послуги на момент бронювання;
- повторна перевірка перетину виконується всередині SQLite-транзакції;
- бізнес-помилки мапляться middleware у `400`, `404`, `409`, `422`;
- стару SQLite-базу сумісно доповнено таблицею `BookingServices` через `CREATE TABLE IF NOT EXISTS`.

## Перевірений HTTP-сценарій

- створено зал `Manual CQRS Hall 2`;
- створено бронювання на 2 години з Projector;
- `HallPrice = 4400`, `ServicesCost = 500`, `TotalPrice = 4900`;
- повторне бронювання того самого інтервалу повернуло `409`;
- заброньований зал виключений із `GET /api/halls/available`.

## Фінальний промпт

"Реалізувати API управління залами та бронюваннями через CQRS + MediatR у Clean Architecture. Команди Create/Update/DeleteHall і BookHall, query GetAvailableHalls, FluentValidation, глобальний middleware помилок, snapshot BookingService, Strategy для ціни, SQLite-транзакція з повторною перевіркою доступності та правильні HTTP-коди. Перевірити build, tests і реальний HTTP flow."
