// PROMPT v2.1: API endpoint бізнес-звіту

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Application.Features.Reports;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConferenceRoomBooking.API.Controllers;

/// <summary>
/// Надає бізнес-звіти та аналітику бронювань.
/// </summary>
[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
  private readonly ISender _sender;

  public ReportsController(ISender sender)
  {
    this._sender = sender;
  }

  /// <summary>
  /// Повертає виручку, бронювання, завантаженість залів і популярність послуг за період.
  /// </summary>
  [HttpGet("business")]
  [Authorize(Roles = "Admin")]
  public async Task<ActionResult<BusinessReportDto>> GetBusinessReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken cancellationToken)
  {
    BusinessReportDto report = await this._sender.Send(new GetBusinessReportQuery(startDate, endDate), cancellationToken);
    return this.Ok(report);
  }
}
