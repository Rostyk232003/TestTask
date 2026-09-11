// PROMPT v1.2.1: Створення каталогу послуг

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Application.Interfaces;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Interfaces;

namespace ConferenceRoomBooking.Application.Services;

/// <summary>
/// Перетворює послуги домену в DTO для API.
/// </summary>
public class ServiceCatalog : IServiceCatalog
{
  private readonly IServiceRepository _serviceRepository;

  public ServiceCatalog(IServiceRepository serviceRepository)
  {
    this._serviceRepository = serviceRepository;
  }

  public async Task<List<ServiceDto>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    List<Service> services = await this._serviceRepository.GetAllAsync(cancellationToken);
    List<ServiceDto> result = new List<ServiceDto>();

    foreach (Service service in services)
    {
      result.Add(new ServiceDto
      {
        Id = service.Id,
        Name = service.Name,
        Price = service.Price,
      });
    }

    return result;
  }
}