// PROMPT v2.1: Director стандартного бізнес-звіту

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Application.Services.Reports;

/// <summary>
/// Керує стандартною послідовністю побудови звіту.
/// </summary>
public class ReportDirector
{
  private readonly IReportBuilder _builder;

  public ReportDirector(IReportBuilder builder)
  {
    this._builder = builder;
  }

  public BusinessReportDto BuildStandardReport(DateTime startDate, DateTime endDate, List<Booking> bookings, List<Hall> halls)
  {
    return this._builder
      .Reset()
      .SetTimePeriod(startDate, endDate)
      .CalculateRevenue(bookings)
      .CalculateOccupancy(bookings, halls)
      .SetMostPopularHall(bookings, halls)
      .SetMostPopularServices(bookings)
      .Build();
  }
}
