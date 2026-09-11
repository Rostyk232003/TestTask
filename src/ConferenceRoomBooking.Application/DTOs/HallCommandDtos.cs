// PROMPT v2.0: DTO команд управління залами

namespace ConferenceRoomBooking.Application.DTOs;

public class CreateHallRequest
{
  public string Name { get; set; } = string.Empty;
  public int Capacity { get; set; }
  public decimal HourlyRate { get; set; }
  public List<Guid> ServiceIds { get; set; } = new List<Guid>();
}

public class UpdateHallRequest : CreateHallRequest
{
  public Guid Id { get; set; }
}

public class AvailableHallDto : HallDto
{
  public List<ServiceDto> AvailableServices { get; set; } = new List<ServiceDto>();
}

public class BookingResponseDto
{
  public Guid BookingId { get; set; }
  public Guid HallId { get; set; }
  public string HallName { get; set; } = string.Empty;
  public DateTime StartTime { get; set; }
  public DateTime EndTime { get; set; }
  public int DurationHours { get; set; }
  public decimal HallPrice { get; set; }
  public decimal ServicesCost { get; set; }
  public decimal TotalPrice { get; set; }
  public List<ServiceDto> SelectedServices { get; set; } = new List<ServiceDto>();
  public string Status { get; set; } = string.Empty;
  public string Message { get; set; } = string.Empty;
}
