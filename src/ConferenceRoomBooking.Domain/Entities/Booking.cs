// PROMPT v1.1: Створення сутності Booking

namespace ConferenceRoomBooking.Domain.Entities;

/// <summary>
/// Представляє бронювання залу на конкретний час.
/// </summary>
public class Booking
{
  private readonly List<BookingService> _bookingServices = new List<BookingService>();

  public Booking(Guid hallId, DateTime startsAt, DateTime endsAt, decimal totalPrice)
  {
    this.Id = Guid.NewGuid();
    this.HallId = hallId;
    this.StartsAt = startsAt;
    this.EndsAt = endsAt;
    this.TotalPrice = totalPrice;
    this.Status = "Booked";
    this.CreatedAt = DateTime.UtcNow;
  }

  private Booking()
  {
    this.Id = Guid.Empty;
    this.HallId = Guid.Empty;
    this.StartsAt = DateTime.UtcNow;
    this.EndsAt = DateTime.UtcNow;
    this.TotalPrice = 0m;
    this.Status = "Booked";
    this.CreatedAt = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }

  public Guid HallId { get; private set; }

  public DateTime StartsAt { get; private set; }

  public DateTime EndsAt { get; private set; }

  public decimal TotalPrice { get; private set; }

  public string Status { get; private set; }

  public DateTime CreatedAt { get; private set; }

  public Hall Hall { get; private set; } = null!;

  public ICollection<BookingService> BookingServices => this._bookingServices;

  /// <summary>
  /// Скасування бронювання.
  /// </summary>
  public void Cancel()
  {
    this.Status = "Cancelled";
  }
}
