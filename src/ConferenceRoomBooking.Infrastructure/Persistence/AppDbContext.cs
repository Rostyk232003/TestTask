// PROMPT v1.1: Створення EF Core DbContext

using ConferenceRoomBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Infrastructure.Persistence;

/// <summary>
/// Контекст бази даних для системи бронювання конференц-залів.
/// </summary>
public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options)
  {
  }

  public DbSet<Hall> Halls { get; set; }

  public DbSet<Service> Services { get; set; }

  public DbSet<HallService> HallServices { get; set; }

  public DbSet<Booking> Bookings { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Hall>(entity =>
    {
      entity.HasKey(x => x.Id);
      entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
      entity.Property(x => x.Capacity).IsRequired();
      entity.Property(x => x.HourlyRate).HasColumnType("decimal(18,2)");
      entity.Property(x => x.CreatedAt).IsRequired();

      entity.HasMany(x => x.HallServices)
              .WithOne(x => x.Hall)
              .HasForeignKey(x => x.HallId)
              .OnDelete(DeleteBehavior.Cascade);

      entity.HasMany(x => x.Bookings)
              .WithOne(x => x.Hall)
              .HasForeignKey(x => x.HallId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<Service>(entity =>
    {
      entity.HasKey(x => x.Id);
      entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
      entity.Property(x => x.Price).HasColumnType("decimal(18,2)");

      entity.HasMany(x => x.HallServices)
              .WithOne(x => x.Service)
              .HasForeignKey(x => x.ServiceId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<HallService>(entity =>
    {
      entity.HasKey(x => x.Id);
      entity.Property(x => x.Price).HasColumnType("decimal(18,2)");
      entity.HasIndex(x => new { x.HallId, x.ServiceId }).IsUnique();
    });

    modelBuilder.Entity<Booking>(entity =>
    {
      entity.HasKey(x => x.Id);
      entity.Property(x => x.StartsAt).IsRequired();
      entity.Property(x => x.EndsAt).IsRequired();
      entity.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");
      entity.Property(x => x.Status).IsRequired().HasMaxLength(50);
    });

    this.SeedData(modelBuilder);
  }

  private void SeedData(ModelBuilder modelBuilder)
  {
    Hall hallA = new Hall("Hall A", 20, 120m);
    Hall hallB = new Hall("Hall B", 35, 180m);
    Hall hallC = new Hall("Hall C", 50, 240m);

    Service projector = new Service("Projector", 40m);
    Service wifi = new Service("Wi-Fi", 15m);
    Service sound = new Service("Sound", 60m);

    modelBuilder.Entity<Hall>().HasData(hallA, hallB, hallC);
    modelBuilder.Entity<Service>().HasData(projector, wifi, sound);
  }
}
