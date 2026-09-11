// PROMPT v1.2: Створення сервісу для розрахунку вартості

namespace ConferenceRoomBooking.Application.Interfaces;

/// <summary>
/// Сервіс для обчислення загальної вартості оренди залу.
/// </summary>
public interface IPricingService
{
  decimal CalculateTotalPrice(decimal baseHourlyRate, DateTime startTime, int durationHours, decimal servicesCost);
}
