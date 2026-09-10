// PROMPT v1.1: Створення реалізації репозиторію Booking

using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Interfaces;
using ConferenceRoomBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Infrastructure.Repositories;

/// <summary>
/// Реалізація репозиторію бронювань на основі EF Core.
/// </summary>
public class BookingRepository : IBookingRepository
{
  private readonly AppDbContext _context;

  public BookingRepository(AppDbContext context)
  {
    this._context = context;
  }

  public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
  {
    await this._context.Bookings.AddAsync(booking, cancellationToken);
    await this._context.SaveChangesAsync(cancellationToken);
  }

  public async Task<List<Booking>> GetByHallAndPeriodAsync(Guid hallId, DateTime startsAt, DateTime endsAt, CancellationToken cancellationToken = default)
  {
    return await this._context.Bookings
        .AsNoTracking()
        .Where(x => x.HallId == hallId && x.Status != "Cancelled" && x.StartsAt < endsAt && x.EndsAt > startsAt)
        .ToListAsync(cancellationToken);
  }

  public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await this._context.Bookings
        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
  }
}
