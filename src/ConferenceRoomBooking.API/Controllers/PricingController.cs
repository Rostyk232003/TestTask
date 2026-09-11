// PROMPT v1.2: Створення контролера для розрахунку вартості

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.API.Controllers;

/// <summary>
/// Контролер для розрахунку вартості оренди залів.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PricingController : ControllerBase
{
  private readonly IPricingService _pricingService;

  public PricingController(IPricingService pricingService)
  {
    this._pricingService = pricingService;
  }

  /// <summary>
  /// Розраховує загальну вартість оренди з урахуванням часу та додаткових послуг.
  /// </summary>
  [HttpPost("calculate")]
  public ActionResult<PricingResponseDto> Calculate([FromBody] PricingRequestDto request)
  {
    if (request.DurationHours <= 0)
    {
      return this.BadRequest("DurationHours must be greater than zero.");
    }

    decimal hallPrice = this._pricingService.CalculateTotalPrice(request.BaseHourlyRate, request.StartTime, request.DurationHours, 0m);
    decimal totalPrice = this._pricingService.CalculateTotalPrice(request.BaseHourlyRate, request.StartTime, request.DurationHours, request.ServicesCost);

    PricingResponseDto response = new PricingResponseDto
    {
      HallPrice = hallPrice,
      ServicesCost = request.ServicesCost,
      TotalPrice = totalPrice,
    };

    return this.Ok(response);
  }
}
