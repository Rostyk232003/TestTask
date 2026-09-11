// PROMPT v2.1: Контракт Builder бізнес-звіту

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Application.Services.Reports;

/// <summary>
/// Покроковий Builder стандартного бізнес-звіту.
/// </summary>
public interface IReportBuilder
{
  IReportBuilder Reset();
  IReportBuilder SetTimePeriod(DateTime startDate, DateTime endDate);
  IReportBuilder CalculateRevenue(List<Booking> bookings);
  IReportBuilder CalculateOccupancy(List<Booking> bookings, List<Hall> halls);
  IReportBuilder SetMostPopularHall(List<Booking> bookings, List<Hall> halls);
  IReportBuilder SetMostPopularServices(List<Booking> bookings);
  BusinessReportDto Build();
}
