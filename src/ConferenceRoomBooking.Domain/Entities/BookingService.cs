// PROMPT v2.0: Створення snapshot сутності послуги бронювання

namespace ConferenceRoomBooking.Domain.Entities;

/// <summary>
/// Послуга, зафіксована в бронюванні разом із ціною на момент створення.
/// </summary>
public class BookingService
{
  public BookingService(Guid bookingId, Guid serviceId, string serviceName, decimal unitPrice)
  {
    this.Id = Guid.NewGuid();
    this.BookingId = bookingId;
    this.ServiceId = serviceId;
    this.ServiceName = serviceName;
    this.UnitPrice = unitPrice;
  }

  private BookingService()
  {
    this.Id = Guid.Empty;
    this.BookingId = Guid.Empty;
    this.ServiceId = Guid.Empty;
    this.ServiceName = string.Empty;
    this.UnitPrice = 0m;
  }

  public Guid Id { get; private set; }

  public Guid BookingId { get; private set; }

  public Guid ServiceId { get; private set; }

  public string ServiceName { get; private set; }

  public decimal UnitPrice { get; private set; }

  public Booking Booking { get; private set; } = null!;

  public Service Service { get; private set; } = null!;
}