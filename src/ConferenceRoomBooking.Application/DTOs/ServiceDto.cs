// PROMPT v1.2.1: Створення DTO для ручного перегляду послуг

namespace ConferenceRoomBooking.Application.DTOs;

/// <summary>
/// DTO додаткової послуги та її вартості.
/// </summary>
public class ServiceDto
{
  public Guid Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public decimal Price { get; set; }
}