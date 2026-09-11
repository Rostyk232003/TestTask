// PROMPT v2.0: CreateHallCommand CQRS

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Application.Exceptions;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Interfaces;
using MediatR;

namespace ConferenceRoomBooking.Application.Features.Halls;

public record CreateHallCommand(string Name, int Capacity, decimal HourlyRate, List<Guid> ServiceIds) : IRequest<HallDto>;

public class CreateHallCommandHandler : IRequestHandler<CreateHallCommand, HallDto>
{
  private readonly IHallRepository _hallRepository;
  private readonly IServiceRepository _serviceRepository;

  public CreateHallCommandHandler(IHallRepository hallRepository, IServiceRepository serviceRepository)
  {
    this._hallRepository = hallRepository;
    this._serviceRepository = serviceRepository;
  }

  public async Task<HallDto> Handle(CreateHallCommand command, CancellationToken cancellationToken)
  {
    Hall? duplicate = await this._hallRepository.GetByNameAsync(command.Name, cancellationToken);
    if (duplicate is not null)
    {
      throw new ConflictException("A hall with this name already exists.");
    }

    List<Service> services = await this._serviceRepository.GetByIdsAsync(command.ServiceIds, cancellationToken);
    if (services.Count != command.ServiceIds.Distinct().Count())
    {
      throw new UnprocessableEntityException("One or more selected services do not exist.");
    }

    Hall hall = new Hall(command.Name, command.Capacity, command.HourlyRate);
    foreach (Service service in services)
    {
      hall.HallServices.Add(new HallService(hall.Id, service.Id, service.Price));
    }

    await this._hallRepository.AddAsync(hall, cancellationToken);
    return new HallDto { Id = hall.Id, Name = hall.Name, Capacity = hall.Capacity, HourlyRate = hall.HourlyRate, IsActive = hall.IsActive };
  }
}
