// PROMPT v1.1: Створення інтерфейсу репозиторію для Hall

using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Domain.Interfaces;

/// <summary>
/// Репозиторій для операцій доступу до залів.
/// </summary>
public interface IHallRepository
{
  Task<Hall?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

  Task<Hall?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

  Task<List<Hall>> GetAllAsync(CancellationToken cancellationToken = default);

  Task AddAsync(Hall hall, CancellationToken cancellationToken = default);

  Task UpdateAsync(Hall hall, CancellationToken cancellationToken = default);

  Task DeleteAsync(Hall hall, CancellationToken cancellationToken = default);
}
