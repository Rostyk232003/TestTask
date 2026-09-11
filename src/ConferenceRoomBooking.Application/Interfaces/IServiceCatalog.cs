// PROMPT v1.2.1: Створення інтерфейсу каталогу послуг

using ConferenceRoomBooking.Application.DTOs;

namespace ConferenceRoomBooking.Application.Interfaces;

/// <summary>
/// Сервіс читання доступних додаткових послуг.
/// </summary>
public interface IServiceCatalog
{
  Task<List<ServiceDto>> GetAllAsync(CancellationToken cancellationToken = default);
}