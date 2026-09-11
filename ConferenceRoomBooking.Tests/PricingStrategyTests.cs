// PROMPT v1.2: Створення unit-тестів для Strategy розрахунку вартості

using ConferenceRoomBooking.Application.Helpers;
using ConferenceRoomBooking.Application.Services;

namespace ConferenceRoomBooking.Tests;

public class PricingStrategyTests
{
  [Fact]
  public void CalculateTotalPrice_WhenStandardHour_ShouldUseBaseRate()
  {
    PricingContext context = new PricingContext(new TimeSlotPricingStrategy());
    decimal baseRate = 2000m;
    DateTime startTime = new DateTime(2026, 1, 1, 10, 0, 0);

    decimal result = context.CalculateTotalPrice(startTime, 1, baseRate, 0m);

    Assert.Equal(2000m, result);
  }

  [Fact]
  public void CalculateTotalPrice_WhenMorningHour_ShouldApplyDiscount()
  {
    PricingContext context = new PricingContext(new TimeSlotPricingStrategy());
    decimal baseRate = 2000m;
    DateTime startTime = new DateTime(2026, 1, 1, 8, 0, 0);

    decimal result = context.CalculateTotalPrice(startTime, 1, baseRate, 0m);

    Assert.Equal(1800m, result);
  }

  [Fact]
  public void CalculateTotalPrice_WhenPeakHour_ShouldApplySurcharge()
  {
    PricingContext context = new PricingContext(new TimeSlotPricingStrategy());
    decimal baseRate = 2000m;
    DateTime startTime = new DateTime(2026, 1, 1, 12, 0, 0);

    decimal result = context.CalculateTotalPrice(startTime, 1, baseRate, 0m);

    Assert.Equal(2300m, result);
  }

  [Fact]
  public void CalculateTotalPrice_WhenEveningHour_ShouldApplyDiscount()
  {
    PricingContext context = new PricingContext(new TimeSlotPricingStrategy());
    decimal baseRate = 2000m;
    DateTime startTime = new DateTime(2026, 1, 1, 20, 0, 0);

    decimal result = context.CalculateTotalPrice(startTime, 1, baseRate, 0m);

    Assert.Equal(1600m, result);
  }

  [Fact]
  public void CalculateTotalPrice_WhenBookingCrossesMultipleSlots_ShouldSumPerHour()
  {
    PricingContext context = new PricingContext(new TimeSlotPricingStrategy());
    decimal baseRate = 2000m;
    DateTime startTime = new DateTime(2026, 1, 1, 11, 30, 0);

    decimal result = context.CalculateTotalPrice(startTime, 3, baseRate, 0m);

    decimal expected = 6600m;
    Assert.Equal(expected, result);
  }

  [Fact]
  public void CalculateTotalPrice_WhenServicesIncluded_ShouldAddServicesCost()
  {
    PricingContext context = new PricingContext(new TimeSlotPricingStrategy());
    decimal baseRate = 2000m;
    DateTime startTime = new DateTime(2026, 1, 1, 09, 0, 0);
    decimal servicesCost = PricingConstants.ProjectorServicePrice + PricingConstants.WiFiServicePrice;

    decimal result = context.CalculateTotalPrice(startTime, 1, baseRate, servicesCost);

    Assert.Equal(2800m, result);
  }
}
