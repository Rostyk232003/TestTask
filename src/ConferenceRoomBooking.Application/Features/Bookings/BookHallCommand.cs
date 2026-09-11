// PROMPT v2.0: BookHallCommand CQRS

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Application.Exceptions;
using ConferenceRoomBooking.Application.Services;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Interfaces;
using MediatR;
using DomainHallService = ConferenceRoomBooking.Domain.Entities.HallService;

namespace ConferenceRoomBooking.Application.Features.Bookings;

public record BookHallCommand(Guid HallId, DateTime StartTime, int DurationHours, List<Guid> ServiceIds) : IRequest<BookingResponseDto>;

public class BookHallCommandHandler : IRequestHandler<BookHallCommand, BookingResponseDto>
{
  private readonly IHallRepository _hallRepository;
  private readonly IBookingRepository _bookingRepository;
  private readonly PricingContext _pricingContext;

  public BookHallCommandHandler(IHallRepository hallRepository, IBookingRepository bookingRepository, PricingContext pricingContext)
  {
    this._hallRepository = hallRepository;
    this._bookingRepository = bookingRepository;
    this._pricingContext = pricingContext;
  }

  public async Task<BookingResponseDto> Handle(BookHallCommand command, CancellationToken cancellationToken)
  {
    Hall? hall = await this._hallRepository.GetByIdAsync(command.HallId, cancellationToken);
    if (hall is null)
    {
      throw new NotFoundException("Hall was not found.");
    }

    if (!hall.IsActive)
    {
      throw new ConflictException("Hall is inactive.");
    }

    DateTime endTime = command.StartTime.AddHours(command.DurationHours);
    bool overlaps = hall.Bookings.Any(x => x.Status != "Cancelled" && x.StartsAt < endTime && x.EndsAt > command.StartTime);
    if (overlaps)
    {
      throw new ConflictException("Hall is already booked for the requested period.");
    }

    List<Guid> distinctServiceIds = command.ServiceIds.Distinct().ToList();
    List<DomainHallService> selectedServices = hall.HallServices.Where(x => distinctServiceIds.Contains(x.ServiceId)).ToList();
    if (selectedServices.Count != distinctServiceIds.Count)
    {
      throw new UnprocessableEntityException("One or more selected services are not available for this hall.");
    }

    decimal servicesCost = selectedServices.Sum(x => x.Price);
    decimal hallPrice = this._pricingContext.CalculateTotalPrice(command.StartTime, command.DurationHours, hall.HourlyRate, 0m);
    decimal totalPrice = this._pricingContext.CalculateTotalPrice(command.StartTime, command.DurationHours, hall.HourlyRate, servicesCost);
    Booking booking = new Booking(hall.Id, command.StartTime, endTime, totalPrice);

    foreach (DomainHallService hallService in selectedServices)
    {
      booking.BookingServices.Add(new BookingService(booking.Id, hallService.ServiceId, hallService.Service.Name, hallService.Price));
    }

    bool saved = await this._bookingRepository.AddIfAvailableAsync(booking, command.StartTime, endTime, cancellationToken);
    if (!saved)
    {
      throw new ConflictException("Hall is already booked for the requested period.");
    }

    BookingResponseDto response = new BookingResponseDto
    {
      BookingId = booking.Id,
      HallId = hall.Id,
      HallName = hall.Name,
      StartTime = command.StartTime,
      EndTime = endTime,
      DurationHours = command.DurationHours,
      HallPrice = hallPrice,
      ServicesCost = servicesCost,
      TotalPrice = totalPrice,
      Status = booking.Status,
      Message = "Booking created successfully.",
    };

    foreach (DomainHallService hallService in selectedServices)
    {
      response.SelectedServices.Add(new ServiceDto { Id = hallService.ServiceId, Name = hallService.Service.Name, Price = hallService.Price });
    }

    return response;
  }
}
