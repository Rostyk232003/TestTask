// PROMPT v1.2: Створення PricingContext для патерну Strategy

using ConferenceRoomBooking.Domain.Interfaces;

namespace ConferenceRoomBooking.Application.Services;

/// <summary>
/// Контекст, який делегує розрахунок конкретній стратегії.
/// </summary>
public class PricingContext
{
  private readonly IPricingStrategy _strategy;

  public PricingContext(IPricingStrategy strategy)
  {
    this._strategy = strategy;
  }

  public decimal CalculateTotalPrice(DateTime startTime, int durationHours, decimal baseHourlyRate, decimal servicesCost)
  {
    decimal hallPrice = this._strategy.CalculateHallPrice(startTime, durationHours, baseHourlyRate);
    decimal totalPrice = hallPrice + servicesCost;
    return totalPrice;
  }
}
