// PROMPT v1.1: Створення інтерфейсу репозиторію для Booking

using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Domain.Interfaces;

/// <summary>
/// Репозиторій для операцій бронювання.
/// </summary>
public interface IBookingRepository
{
  Task AddAsync(Booking booking, CancellationToken cancellationToken = default);

  Task<List<Booking>> GetByHallAndPeriodAsync(Guid hallId, DateTime startsAt, DateTime endsAt, CancellationToken cancellationToken = default);

  Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
