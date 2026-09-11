// PROMPT v1.2.1: Синхронізація початкових даних для ручної перевірки

using ConferenceRoomBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Infrastructure.Persistence;

/// <summary>
/// Створює або оновлює демонстраційні дані про зали та послуги.
/// </summary>
public class DatabaseSeeder
{
  private readonly AppDbContext _context;

  public DatabaseSeeder(AppDbContext context)
  {
    this._context = context;
  }

  public async Task SeedAsync(CancellationToken cancellationToken = default)
  {
    await this._context.Database.ExecuteSqlRawAsync(
      "CREATE TABLE IF NOT EXISTS BookingServices (Id TEXT NOT NULL CONSTRAINT PK_BookingServices PRIMARY KEY, BookingId TEXT NOT NULL, ServiceId TEXT NOT NULL, ServiceName TEXT NOT NULL, UnitPrice decimal(18,2) NOT NULL, CONSTRAINT FK_BookingServices_Bookings_BookingId FOREIGN KEY (BookingId) REFERENCES Bookings (Id) ON DELETE CASCADE, CONSTRAINT FK_BookingServices_Services_ServiceId FOREIGN KEY (ServiceId) REFERENCES Services (Id) ON DELETE RESTRICT);",
      cancellationToken);

    await this.UpsertHallAsync("Hall A", 50, 2000m, cancellationToken);
    await this.UpsertHallAsync("Hall B", 100, 3500m, cancellationToken);
    await this.UpsertHallAsync("Hall C", 30, 1500m, cancellationToken);

    await this.UpsertServiceAsync("Projector", 500m, cancellationToken);
    await this.UpsertServiceAsync("Wi-Fi", 300m, cancellationToken);
    await this.UpsertServiceAsync("Sound", 700m, cancellationToken);

    await this._context.SaveChangesAsync(cancellationToken);
  }

  private async Task UpsertHallAsync(string name, int capacity, decimal hourlyRate, CancellationToken cancellationToken)
  {
    Hall? hall = await this._context.Halls.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

    if (hall is null)
    {
      await this._context.Halls.AddAsync(new Hall(name, capacity, hourlyRate), cancellationToken);
      return;
    }

    hall.Update(name, capacity, hourlyRate);
  }

  private async Task UpsertServiceAsync(string name, decimal price, CancellationToken cancellationToken)
  {
    Service? service = await this._context.Services.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

    if (service is null)
    {
      await this._context.Services.AddAsync(new Service(name, price), cancellationToken);
      return;
    }

    service.Update(name, price);
  }
}