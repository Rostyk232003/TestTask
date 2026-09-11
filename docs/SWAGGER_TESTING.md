# Swagger UI: ручна перевірка API

## 1. Запуск

У PowerShell з кореня проєкту:

```powershell
dotnet build ConferenceRoomBooking.sln
dotnet run --project src/ConferenceRoomBooking.API --urls http://localhost:5090
```

Відкрийте у браузері:

```text
http://localhost:5090/swagger
```

Застосунок автоматично створює або оновлює SQLite-базу та додає початкові дані.

## 2. Отримання JWT

Endpoint `POST /api/auth/token` не потребує авторизації.

У Swagger відкрийте endpoint, натисніть **Try it out**, використайте:

```json
{
  "username": "admin",
  "password": "ChangeThisDevelopmentPassword!"
}
```

Скопіюйте значення `accessToken` з відповіді.

У верхній частині Swagger натисніть **Authorize**, введіть:

```text
Bearer <accessToken>
```

Після цього натисніть **Authorize** і **Close**. Захищені endpoints тепер надсилатимуть JWT автоматично.

> Demo credentials призначені лише для локальної перевірки. Для production ключ і пароль не повинні зберігатися в `appsettings.json`.

## 3. Перевірка початкових даних

### Список залів

Викличте `GET /api/halls`.

Очікувані зали:

- Hall A: capacity `50`, hourly rate `2000`;
- Hall B: capacity `100`, hourly rate `3500`;
- Hall C: capacity `30`, hourly rate `1500`.

Збережіть `id` залу, наприклад Hall A, для наступних запитів.

### Список послуг

Викличте `GET /api/services`.

Очікувані послуги:

- Projector: `500`;
- Wi-Fi: `300`;
- Sound: `700`.

Збережіть `id` Projector, Wi-Fi і Sound.

## 4. Створення залу

Викличте `POST /api/halls` з JWT:

```json
{
  "name": "Swagger Test Hall",
  "capacity": 60,
  "hourlyRate": 2200,
  "serviceIds": [
    "b77393a7-a368-49fb-ae51-b43a3c4a4bca",
    "0d807f28-2976-4c81-b182-5db457c51819"
  ]
}
```

Очікуваний результат: `201 Created` і унікальний `id` залу.

Збережіть цей `id` для редагування, пошуку доступності та бронювання.

Негативна перевірка: повторіть запит із тією самою назвою. Очікується `409 Conflict`.

## 5. Редагування залу

Викличте `PUT /api/halls/{id}`:

```json
{
  "name": "Swagger Test Hall Updated",
  "capacity": 70,
  "hourlyRate": 2500,
  "serviceIds": [
    "b77393a7-a368-49fb-ae51-b43a3c4a4bca",
    "0d807f28-2976-4c81-b182-5db457c51819",
    "9492a6e0-8ee7-419c-a244-a02e379c2bcd"
  ]
}
```

Очікується `200 OK`. Перевірте, що ціна, місткість і список послуг оновлені.

## 6. Пошук доступних залів

Викличте `GET /api/halls/available` і задайте query parameters:

```text
startTime = 2026-09-20T10:00:00Z
endTime   = 2026-09-20T14:00:00Z
capacity  = 50
```

Очікується список активних залів місткістю не менше 50 осіб, які не мають бронювання з перетином цього інтервалу.

Перевірка валідації: передайте `endTime` раніше за `startTime`. Очікується `400 Bad Request`.

## 7. Перевірка розрахунку вартості

Викличте `POST /api/pricing/calculate`. Для залу з базовою ставкою `2000` перевірте такі запити.

### Ранкова година: -10%

```json
{
  "baseHourlyRate": 2000,
  "startTime": "2026-09-20T08:00:00Z",
  "durationHours": 1,
  "servicesCost": 0
}
```

Очікується `hallPrice = 1800`, `totalPrice = 1800`.

### Стандартна година

Для `10:00` очікується `hallPrice = 2000`.

### Пікова година: +15%

Для `12:00` очікується `hallPrice = 2300`.

### Вечірня година: -20%

Для `20:00` очікується `hallPrice = 1600`.

### Послуги

Для стандартної години й Projector + Wi-Fi передайте `servicesCost = 800`.

Очікується:

```text
hallPrice = 2000
servicesCost = 800
totalPrice = 2800
```

### Перетин часових слотів

Для `startTime = 11:30`, `durationHours = 3`, `baseHourlyRate = 2000` очікується `hallPrice = 6600`.

## 8. Бронювання

Викличте `POST /api/bookings` з JWT:

```json
{
  "hallId": "<hall-id>",
  "startTime": "2026-09-20T10:00:00Z",
  "durationHours": 2,
  "serviceIds": ["<projector-id>"]
}
```

Очікується `201 Created` і відповідь із:

- `bookingId`;
- `hallPrice`;
- `servicesCost`;
- `totalPrice`;
- `selectedServices`;
- `status = Booked`.

Для залу зі ставкою `2200` і Projector очікується:

```text
hallPrice = 4400
servicesCost = 500
totalPrice = 4900
```

Повторіть такий самий запит. Очікується `409 Conflict`, оскільки часовий інтервал уже зайнятий.

Після бронювання повторіть `GET /api/halls/available` для цього інтервалу. Заброньований зал не повинен бути у відповіді.

## 9. Бізнес-звіт

Спочатку отримайте Admin JWT і натисніть **Authorize**.

Викличте `GET /api/reports/business`:

```text
startDate = 2026-09-01T00:00:00Z
endDate   = 2026-10-01T00:00:00Z
```

Очікувана відповідь містить:

- `totalRevenue` — сума `Booking.TotalPrice`;
- `totalBookings` — кількість активних бронювань;
- `occupancyRatePercentage` — завантаженість за робочим часом `06:00–23:00`;
- `hallOccupancy` — статистика кожного активного залу;
- `mostPopularHallName`;
- `servicePopularity` — використання послуг і їхня виручка;
- `workingHoursSlotStats`.

Скасовані бронювання не повинні впливати на звіт.

## 10. Видалення залу

Викличте `DELETE /api/halls/{id}` з Admin JWT.

Очікується `204 No Content`. Видалення є soft delete: зал стає неактивним і не повертається у доступних залах.

Якщо зал має активне майбутнє бронювання, очікується `409 Conflict`.

## 11. Перевірка помилок

Перевірте такі сценарії:

| Сценарій                                | Очікуваний статус |
| --------------------------------------- | ----------------: |
| Захищений endpoint без JWT              |             `401` |
| Невалідні дані запиту                   |             `400` |
| Зал не знайдено                         |             `404` |
| Перетин бронювання або дубльована назва |             `409` |
| Послуга не доступна для залу            |             `422` |
| Непередбачена помилка сервера           |             `500` |

## 12. Автоматична перевірка

З кореня проєкту:

```powershell
dotnet test ConferenceRoomBooking.sln --nologo
```

Тести охоплюють Strategy, Builder, JWT authorization і API integration flow.
