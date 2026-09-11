// PROMPT v1.2: Створення FlatPricingStrategy

using ConferenceRoomBooking.Domain.Interfaces;

namespace ConferenceRoomBooking.Application.Services;

/// <summary>
/// Базова стратегія без часових модифікаторів.
/// </summary>
public class FlatPricingStrategy : IPricingStrategy
{
  public decimal CalculateHallPrice(DateTime startTime, int durationHours, decimal baseHourlyRate)
  {
    if (durationHours <= 0)
    {
      return 0m;
    }

    decimal totalPrice = baseHourlyRate * durationHours;
    return totalPrice;
  }
}
