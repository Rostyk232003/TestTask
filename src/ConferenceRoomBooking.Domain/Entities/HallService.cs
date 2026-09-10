// PROMPT v1.1: Створення сутності HallService

namespace ConferenceRoomBooking.Domain.Entities;

/// <summary>
/// Зв'язок між залом та послугою із відслідковуванням ціни.
/// </summary>
public class HallService
{
  public HallService(Guid hallId, Guid serviceId, decimal price)
  {
    this.Id = Guid.NewGuid();
    this.HallId = hallId;
    this.ServiceId = serviceId;
    this.Price = price;
  }

  private HallService()
  {
    this.Id = Guid.Empty;
    this.HallId = Guid.Empty;
    this.ServiceId = Guid.Empty;
    this.Price = 0m;
  }

  public Guid Id { get; private set; }

  public Guid HallId { get; private set; }

  public Guid ServiceId { get; private set; }

  public decimal Price { get; private set; }

  public Hall Hall { get; private set; } = null!;

  public Service Service { get; private set; } = null!;
}
