// PROMPT v1.1: Створення сутності Hall

using System.Collections.Generic;

namespace ConferenceRoomBooking.Domain.Entities;

/// <summary>
/// Представляє конференц-зал у системі бронювання.
/// </summary>
public class Hall
{
  private readonly List<HallService> _hallServices;
  private readonly List<Booking> _bookings;

  public Hall(string name, int capacity, decimal hourlyRate)
  {
    this.Id = Guid.NewGuid();
    this.Name = name;
    this.Capacity = capacity;
    this.HourlyRate = hourlyRate;
    this.IsActive = true;
    this.CreatedAt = DateTime.UtcNow;
    this._hallServices = new List<HallService>();
    this._bookings = new List<Booking>();
  }

  private Hall()
  {
    this.Id = Guid.Empty;
    this.Name = string.Empty;
    this.Capacity = 0;
    this.HourlyRate = 0m;
    this.IsActive = true;
    this.CreatedAt = DateTime.UtcNow;
    this._hallServices = new List<HallService>();
    this._bookings = new List<Booking>();
  }

  public Guid Id { get; private set; }

  public string Name { get; private set; }

  public int Capacity { get; private set; }

  public decimal HourlyRate { get; private set; }

  public bool IsActive { get; private set; }

  public DateTime CreatedAt { get; private set; }

  public ICollection<HallService> HallServices => this._hallServices;

  public ICollection<Booking> Bookings => this._bookings;

  /// <summary>
  /// Оновлює основні характеристики залу.
  /// </summary>
  public void Update(string name, int capacity, decimal hourlyRate)
  {
    this.Name = name;
    this.Capacity = capacity;
    this.HourlyRate = hourlyRate;
  }

  /// <summary>
  /// Встановлює статус активності залу.
  /// </summary>
  public void SetActive(bool isActive)
  {
    this.IsActive = isActive;
  }
}
