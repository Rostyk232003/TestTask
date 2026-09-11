// PROMPT v1.1: Створення реалізації репозиторію Hall

using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Interfaces;
using ConferenceRoomBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Infrastructure.Repositories;

/// <summary>
/// Реалізація репозиторію залів на основі EF Core.
/// </summary>
public class HallRepository : IHallRepository
{
  private readonly AppDbContext _context;

  public HallRepository(AppDbContext context)
  {
    this._context = context;
  }

  public async Task<Hall?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await this._context.Halls
        .Include(x => x.HallServices)
        .ThenInclude(x => x.Service)
        .Include(x => x.Bookings)
        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
  }

  public async Task<Hall?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
  {
    return await this._context.Halls.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
  }

  public async Task<List<Hall>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    return await this._context.Halls
        .AsNoTracking()
        .Include(x => x.HallServices)
        .ThenInclude(x => x.Service)
        .Include(x => x.Bookings)
        .OrderBy(x => x.Name)
        .ToListAsync(cancellationToken);
  }

  public async Task AddAsync(Hall hall, CancellationToken cancellationToken = default)
  {
    await this._context.Halls.AddAsync(hall, cancellationToken);
    await this._context.SaveChangesAsync(cancellationToken);
  }

  public async Task UpdateAsync(Hall hall, CancellationToken cancellationToken = default)
  {
    this._context.Halls.Update(hall);
    await this._context.SaveChangesAsync(cancellationToken);
  }

  public async Task DeleteAsync(Hall hall, CancellationToken cancellationToken = default)
  {
    this._context.Halls.Remove(hall);
    await this._context.SaveChangesAsync(cancellationToken);
  }
}
