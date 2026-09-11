// PROMPT v1.1: Створення контролера Halls

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Application.Interfaces;
using ConferenceRoomBooking.Application.Features.Halls;
using MediatR;
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
  private readonly ISender _sender;

  public HallsController(IHallService hallService, ISender sender)
  {
    this._hallService = hallService;
    this._sender = sender;
  }

  /// <summary>
  /// Створює конференц-зал.
  /// </summary>
  [HttpPost]
  public async Task<ActionResult<HallDto>> Create(CreateHallRequest request, CancellationToken cancellationToken)
  {
    HallDto result = await this._sender.Send(new CreateHallCommand(request.Name, request.Capacity, request.HourlyRate, request.ServiceIds), cancellationToken);
    return this.CreatedAtAction(nameof(this.GetById), new { id = result.Id }, result);
  }

  /// <summary>
  /// Оновлює конференц-зал.
  /// </summary>
  [HttpPut("{id:guid}")]
  public async Task<ActionResult<HallDto>> Update(Guid id, UpdateHallRequest request, CancellationToken cancellationToken)
  {
    HallDto result = await this._sender.Send(new UpdateHallCommand(id, request.Name, request.Capacity, request.HourlyRate, request.ServiceIds), cancellationToken);
    return this.Ok(result);
  }

  /// <summary>
  /// Деактивує конференц-зал.
  /// </summary>
  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
  {
    await this._sender.Send(new DeleteHallCommand(id), cancellationToken);
    return this.NoContent();
  }

  /// <summary>
  /// Повертає доступні зали за інтервалом і місткістю.
  /// </summary>
  [HttpGet("available")]
  public async Task<ActionResult<List<AvailableHallDto>>> GetAvailable([FromQuery] DateTime startTime, [FromQuery] DateTime endTime, [FromQuery] int capacity, CancellationToken cancellationToken)
  {
    List<AvailableHallDto> result = await this._sender.Send(new GetAvailableHallsQuery(startTime, endTime, capacity), cancellationToken);
    return this.Ok(result);
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
