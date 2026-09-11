// PROMPT v2.0: API controller бронювання через MediatR

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Application.Features.Bookings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.API.Controllers;

/// <summary>
/// Контролер бронювання конференц-залів.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
  private readonly ISender _sender;

  public BookingsController(ISender sender)
  {
    this._sender = sender;
  }

  /// <summary>
  /// Створює бронювання з вибраними послугами та розрахунком вартості.
  /// </summary>
  [HttpPost]
  public async Task<ActionResult<BookingResponseDto>> Create([FromBody] BookHallCommand command, CancellationToken cancellationToken)
  {
    BookingResponseDto result = await this._sender.Send(command, cancellationToken);
    return this.Created($"api/bookings/{result.BookingId}", result);
  }
}