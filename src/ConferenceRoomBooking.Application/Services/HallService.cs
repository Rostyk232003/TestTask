// PROMPT v1.1: Створення сервісу HallService

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Application.Interfaces;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Interfaces;

namespace ConferenceRoomBooking.Application.Services;

/// <summary>
/// Сервіс для бізнес-операцій над залами.
/// </summary>
public class HallService : IHallService
{
  private readonly IHallRepository _hallRepository;

  public HallService(IHallRepository hallRepository)
  {
    this._hallRepository = hallRepository;
  }

  public async Task<List<HallDto>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    List<Hall> halls = await this._hallRepository.GetAllAsync(cancellationToken);
    List<HallDto> result = new List<HallDto>();

    foreach (Hall hall in halls)
    {
      if (hall.IsActive)
      {
        result.Add(new HallDto
        {
          Id = hall.Id,
          Name = hall.Name,
          Capacity = hall.Capacity,
          HourlyRate = hall.HourlyRate,
          IsActive = hall.IsActive,
        });
      }
    }

    return result;
  }

  public async Task<HallDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    Hall? hall = await this._hallRepository.GetByIdAsync(id, cancellationToken);

    if (hall is null || !hall.IsActive)
    {
      return null;
    }

    return new HallDto
    {
      Id = hall.Id,
      Name = hall.Name,
      Capacity = hall.Capacity,
      HourlyRate = hall.HourlyRate,
      IsActive = hall.IsActive,
    };
  }
}
