// PROMPT v1.2.1: Створення endpoint каталогу послуг

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRoomBooking.API.Controllers;

/// <summary>
/// Контролер доступних додаткових послуг.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
  private readonly IServiceCatalog _serviceCatalog;

  public ServicesController(IServiceCatalog serviceCatalog)
  {
    this._serviceCatalog = serviceCatalog;
  }

  /// <summary>
  /// Повертає послуги та їхні ціни.
  /// </summary>
  [HttpGet]
  public async Task<ActionResult<List<ServiceDto>>> GetAll(CancellationToken cancellationToken)
  {
    List<ServiceDto> services = await this._serviceCatalog.GetAllAsync(cancellationToken);
    return this.Ok(services);
  }
}