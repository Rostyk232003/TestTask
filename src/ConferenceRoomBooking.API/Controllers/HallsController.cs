// PROMPT v1.1: Створення контролера Halls

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.API.Controllers;

/// <summary>
/// Контролер для управління залами.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HallsController : ControllerBase
{
  private readonly IHallService _hallService;

  public HallsController(IHallService hallService)
  {
    this._hallService = hallService;
  }

  /// <summary>
  /// Повертає список активних залів.
  /// </summary>
  [HttpGet]
  public async Task<ActionResult<List<HallDto>>> GetAll(CancellationToken cancellationToken)
  {
    List<HallDto> halls = await this._hallService.GetAllAsync(cancellationToken);
    return this.Ok(halls);
  }

  /// <summary>
  /// Повертає зал за ідентифікатором.
  /// </summary>
  [HttpGet("{id:guid}")]
  public async Task<ActionResult<HallDto>> GetById(Guid id, CancellationToken cancellationToken)
  {
    HallDto? hall = await this._hallService.GetByIdAsync(id, cancellationToken);

    if (hall is null)
    {
      return this.NotFound();
    }

    return this.Ok(hall);
  }
}
