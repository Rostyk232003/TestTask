// PROMPT v1.2: Створення TimeSlotPricingStrategy

using ConferenceRoomBooking.Application.Helpers;
using ConferenceRoomBooking.Domain.Interfaces;

namespace ConferenceRoomBooking.Application.Services;

/// <summary>
/// Стратегія з часовими правилами: ранок, стандарт, пік, вечір.
/// </summary>
public class TimeSlotPricingStrategy : IPricingStrategy
{
  public decimal CalculateHallPrice(DateTime startTime, int durationHours, decimal baseHourlyRate)
  {
    if (durationHours <= 0)
    {
      return 0m;
    }

    decimal totalPrice = 0m;
    DateTime currentTime = startTime;
    DateTime endTime = startTime.AddHours(durationHours);

    while (currentTime < endTime)
    {
      DateTime nextBoundary = this.GetNextBoundary(currentTime);
      DateTime segmentEnd = endTime < nextBoundary ? endTime : nextBoundary;
      double segmentHours = (segmentEnd - currentTime).TotalHours;
      decimal multiplier = this.GetMultiplierForTime(currentTime);
      totalPrice += (decimal)segmentHours * baseHourlyRate * multiplier;
      currentTime = segmentEnd;
    }

    return totalPrice;
  }

  private DateTime GetNextBoundary(DateTime currentTime)
  {
    DateTime nextHourBoundary = currentTime.Date.AddHours(Math.Floor(currentTime.Hour + 1d));

    List<DateTime> boundaryPoints = new List<DateTime>
        {
            currentTime.Date.AddHours(6),
            currentTime.Date.AddHours(9),
            currentTime.Date.AddHours(12),
            currentTime.Date.AddHours(14),
            currentTime.Date.AddHours(18),
            currentTime.Date.AddHours(23),
        };

    DateTime nextBoundary = boundaryPoints
        .Where(x => x > currentTime)
        .OrderBy(x => x)
        .FirstOrDefault();

    if (nextBoundary == default)
    {
      nextBoundary = nextHourBoundary;
    }

    if (nextHourBoundary > currentTime && (nextBoundary == default || nextHourBoundary < nextBoundary))
    {
      return nextHourBoundary;
    }

    return nextBoundary;
  }

  private decimal GetMultiplierForTime(DateTime time)
  {
    int hour = time.Hour;

    if (hour >= 6 && hour < 9)
    {
      return 1m - PricingConstants.MorningDiscountRate;
    }

    if (hour >= 12 && hour < 14)
    {
      return 1m + PricingConstants.PeakSurchargeRate;
    }

    if (hour >= 18 && hour < 23)
    {
      return 1m - PricingConstants.EveningDiscountRate;
    }

    return 1m + PricingConstants.StandardRate;
  }
}
