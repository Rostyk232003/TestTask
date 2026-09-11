// PROMPT v2.1: Контракт read model аналітики у Domain layer

using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Domain.Interfaces;

/// <summary>
/// Read-only джерело даних для бізнес-аналітики.
/// </summary>
public interface IAnalyticsRepository
{
  Task<AnalyticsData> GetAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}

/// <summary>
/// Дані, необхідні Builder для формування звіту.
/// </summary>
public class AnalyticsData
{
  public List<Hall> Halls { get; set; } = new List<Hall>();
  public List<Booking> Bookings { get; set; } = new List<Booking>();
}