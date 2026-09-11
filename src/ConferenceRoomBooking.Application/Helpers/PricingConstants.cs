// PROMPT v1.2: Створення констант для правил ціноутворення

namespace ConferenceRoomBooking.Application.Helpers;

/// <summary>
/// Константи, які використовуються для розрахунку вартості оренди.
/// </summary>
public static class PricingConstants
{
  public const decimal MorningDiscountRate = 0.10m;

  public const decimal StandardRate = 0.00m;

  public const decimal PeakSurchargeRate = 0.15m;

  public const decimal EveningDiscountRate = 0.20m;

  public const decimal ProjectorServicePrice = 500m;

  public const decimal WiFiServicePrice = 300m;

  public const decimal SoundServicePrice = 700m;
}
