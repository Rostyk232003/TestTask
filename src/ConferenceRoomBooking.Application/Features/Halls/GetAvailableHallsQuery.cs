// PROMPT v2.0: GetAvailableHallsQuery CQRS

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Interfaces;
using MediatR;

namespace ConferenceRoomBooking.Application.Features.Halls;

public record GetAvailableHallsQuery(DateTime StartTime, DateTime EndTime, int Capacity) : IRequest<List<AvailableHallDto>>;

public class GetAvailableHallsQueryHandler : IRequestHandler<GetAvailableHallsQuery, List<AvailableHallDto>>
{
  private readonly IHallRepository _hallRepository;

  public GetAvailableHallsQueryHandler(IHallRepository hallRepository)
  {
    this._hallRepository = hallRepository;
  }

  public async Task<List<AvailableHallDto>> Handle(GetAvailableHallsQuery query, CancellationToken cancellationToken)
  {
    List<Hall> halls = await this._hallRepository.GetAllAsync(cancellationToken);
    List<AvailableHallDto> result = new List<AvailableHallDto>();

    foreach (Hall hall in halls)
    {
      bool overlaps = hall.Bookings.Any(x => x.Status != "Cancelled" && x.StartsAt < query.EndTime && x.EndsAt > query.StartTime);
      if (!hall.IsActive || hall.Capacity < query.Capacity || overlaps)
      {
        continue;
      }

      AvailableHallDto dto = new AvailableHallDto
      {
        Id = hall.Id,
        Name = hall.Name,
        Capacity = hall.Capacity,
        HourlyRate = hall.HourlyRate,
        IsActive = hall.IsActive,
      };

      foreach (HallService hallService in hall.HallServices)
      {
        dto.AvailableServices.Add(new ServiceDto
        {
          Id = hallService.ServiceId,
          Name = hallService.Service.Name,
          Price = hallService.Price,
        });
      }

      result.Add(dto);
    }

    return result;
  }
}
