// PROMPT v1.2.1: Створення інтерфейсу репозиторію послуг для ручної перевірки

using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Domain.Interfaces;

/// <summary>
/// Репозиторій для отримання додаткових послуг.
/// </summary>
public interface IServiceRepository
{
  Task<List<Service>> GetAllAsync(CancellationToken cancellationToken = default);

  Task<List<Service>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}