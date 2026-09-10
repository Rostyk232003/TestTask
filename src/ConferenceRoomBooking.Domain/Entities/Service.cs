// PROMPT v1.1: Створення сутності Service

using System.Collections.Generic;

namespace ConferenceRoomBooking.Domain.Entities;

/// <summary>
/// Представляє послугу, що може бути прив'язана до залу.
/// </summary>
public class Service
{
  private readonly List<HallService> _hallServices;

  public Service(string name, decimal price)
  {
    this.Id = Guid.NewGuid();
    this.Name = name;
    this.Price = price;
    this._hallServices = new List<HallService>();
  }

  private Service()
  {
    this.Id = Guid.Empty;
    this.Name = string.Empty;
    this.Price = 0m;
    this._hallServices = new List<HallService>();
  }

  public Guid Id { get; private set; }

  public string Name { get; private set; }

  public decimal Price { get; private set; }

  public ICollection<HallService> HallServices => this._hallServices;

  /// <summary>
  /// Оновлює назву та ціну послуги.
  /// </summary>
  public void Update(string name, decimal price)
  {
    this.Name = name;
    this.Price = price;
  }
}
