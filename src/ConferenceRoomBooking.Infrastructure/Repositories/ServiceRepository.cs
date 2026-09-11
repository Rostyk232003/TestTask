// PROMPT v1.2.1: Створення реалізації репозиторію послуг

using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Interfaces;
using ConferenceRoomBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Infrastructure.Repositories;

/// <summary>
/// Реалізація репозиторію послуг на основі EF Core.
/// </summary>
public class ServiceRepository : IServiceRepository
{
  private readonly AppDbContext _context;

  public ServiceRepository(AppDbContext context)
  {
    this._context = context;
  }

  public async Task<List<Service>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    return await this._context.Services
      .AsNoTracking()
      .OrderBy(x => x.Name)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<Service>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
  {
    List<Guid> serviceIds = ids.Distinct().ToList();
    return await this._context.Services
      .Where(x => serviceIds.Contains(x.Id))
      .ToListAsync(cancellationToken);
  }
}