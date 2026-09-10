// PROMPT v1.1: Створення DTO для Hall

namespace ConferenceRoomBooking.Application.DTOs;

/// <summary>
/// DTO для відображення даних залу.
/// </summary>
public class HallDto
{
  public Guid Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public int Capacity { get; set; }

  public decimal HourlyRate { get; set; }

  public bool IsActive { get; set; }
}
