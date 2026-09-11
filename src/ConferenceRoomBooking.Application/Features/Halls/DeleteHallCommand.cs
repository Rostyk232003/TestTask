// PROMPT v2.0: DeleteHallCommand CQRS

using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Interfaces;
using ConferenceRoomBooking.Application.Exceptions;
using MediatR;

namespace ConferenceRoomBooking.Application.Features.Halls;

public record DeleteHallCommand(Guid Id) : IRequest;

public class DeleteHallCommandHandler : IRequestHandler<DeleteHallCommand>
{
  private readonly IHallRepository _hallRepository;

  public DeleteHallCommandHandler(IHallRepository hallRepository)
  {
    this._hallRepository = hallRepository;
  }

  public async Task Handle(DeleteHallCommand command, CancellationToken cancellationToken)
  {
    Hall? hall = await this._hallRepository.GetByIdAsync(command.Id, cancellationToken);
    if (hall is null)
    {
      throw new NotFoundException("Hall was not found.");
    }

    bool hasFutureBookings = hall.Bookings.Any(x => x.Status != BookingStatus.Cancelled && x.EndsAt > DateTime.UtcNow);
    if (hasFutureBookings)
    {
      throw new ConflictException("Hall has active future bookings.");
    }

    hall.SetActive(false);
    await this._hallRepository.UpdateAsync(hall, cancellationToken);
  }
}
