// PROMPT v1.2: Створення DTO для запиту розрахунку ціни

namespace ConferenceRoomBooking.Application.DTOs;

/// <summary>
/// Запит на розрахунок вартості оренди залу.
/// </summary>
public class PricingRequestDto
{
  public decimal BaseHourlyRate { get; set; }

  public DateTime StartTime { get; set; }

  public int DurationHours { get; set; }

  public decimal ServicesCost { get; set; }
}
