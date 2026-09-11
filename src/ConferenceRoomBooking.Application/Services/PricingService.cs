// PROMPT v1.2: Створення PricingService для розрахунку вартості

using ConferenceRoomBooking.Application.Interfaces;

namespace ConferenceRoomBooking.Application.Services;

/// <summary>
/// Сервіс, який делегує розрахунок конкретній стратегії ціноутворення.
/// </summary>
public class PricingService : IPricingService
{
  private readonly PricingContext _pricingContext;

  public PricingService(PricingContext pricingContext)
  {
    this._pricingContext = pricingContext;
  }

  public decimal CalculateTotalPrice(decimal baseHourlyRate, DateTime startTime, int durationHours, decimal servicesCost)
  {
    return this._pricingContext.CalculateTotalPrice(startTime, durationHours, baseHourlyRate, servicesCost);
  }
}
