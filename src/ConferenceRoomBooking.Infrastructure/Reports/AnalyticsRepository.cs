// PROMPT v2.1: EF Core read model аналітики

using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Interfaces;
using ConferenceRoomBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Infrastructure.Reports;

/// <summary>
/// Отримує read-only дані для аналітичних звітів.
/// </summary>
public class AnalyticsRepository : IAnalyticsRepository
{
  private readonly AppDbContext _context;

  public AnalyticsRepository(AppDbContext context)
  {
    this._context = context;
  }

  public async Task<AnalyticsData> GetAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
  {
    List<Hall> halls = await this._context.Halls
      .AsNoTracking()
      .Where(x => x.IsActive)
      .ToListAsync(cancellationToken);

    List<Booking> bookings = await this._context.Bookings
      .AsNoTracking()
      .Include(x => x.BookingServices)
      .Where(x => x.StartsAt < endDate && x.EndsAt > startDate && x.Status != BookingStatus.Cancelled)
      .ToListAsync(cancellationToken);

    return new AnalyticsData
    {
      Halls = halls,
      Bookings = bookings,
    };
  }
}
