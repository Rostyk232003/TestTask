// PROMPT v1.2: Створення DTO для відповіді розрахунку ціни

namespace ConferenceRoomBooking.Application.DTOs;

/// <summary>
/// Відповідь з підсумковою вартістю оренди та послуг.
/// </summary>
public class PricingResponseDto
{
  public decimal TotalPrice { get; set; }

  public decimal HallPrice { get; set; }

  public decimal ServicesCost { get; set; }
}
