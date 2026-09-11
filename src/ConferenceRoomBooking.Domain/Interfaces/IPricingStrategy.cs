// PROMPT v1.2: Створення інтерфейсу Strategy для розрахунку вартості

namespace ConferenceRoomBooking.Domain.Interfaces;

/// <summary>
/// Стратегія розрахунку вартості оренди залу на основі часу бронювання.
/// </summary>
public interface IPricingStrategy
{
  decimal CalculateHallPrice(DateTime startTime, int durationHours, decimal baseHourlyRate);
}
