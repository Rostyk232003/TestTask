// PROMPT v1.1: Створення інтерфейсу сервісу для Hall

using ConferenceRoomBooking.Application.DTOs;

namespace ConferenceRoomBooking.Application.Interfaces;

/// <summary>
/// Сервіс для бізнес-операцій над залами.
/// </summary>
public interface IHallService
{
  Task<List<HallDto>> GetAllAsync(CancellationToken cancellationToken = default);

  Task<HallDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
