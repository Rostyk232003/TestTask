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

  public DbSet<BookingService> BookingServices { get; set; }

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
      entity.Property(x => x.Status).HasConversion<string>();

      entity.HasMany(x => x.BookingServices)
        .WithOne(x => x.Booking)
        .HasForeignKey(x => x.BookingId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<BookingService>(entity =>
    {
      entity.HasKey(x => x.Id);
      entity.Property(x => x.ServiceName).IsRequired().HasMaxLength(200);
      entity.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
      entity.HasOne(x => x.Service)
        .WithMany()
        .HasForeignKey(x => x.ServiceId)
        .OnDelete(DeleteBehavior.Restrict);
    });

    this.SeedData(modelBuilder);
  }

  private void SeedData(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Hall>().HasData(
      new { Id = SeedIds.HallA, Name = "Hall A", Capacity = 50, HourlyRate = 2000m, IsActive = true, CreatedAt = new DateTime(2026, 9, 10, 0, 0, 0, DateTimeKind.Utc) },
      new { Id = SeedIds.HallB, Name = "Hall B", Capacity = 100, HourlyRate = 3500m, IsActive = true, CreatedAt = new DateTime(2026, 9, 10, 0, 0, 0, DateTimeKind.Utc) },
      new { Id = SeedIds.HallC, Name = "Hall C", Capacity = 30, HourlyRate = 1500m, IsActive = true, CreatedAt = new DateTime(2026, 9, 10, 0, 0, 0, DateTimeKind.Utc) });

    modelBuilder.Entity<Service>().HasData(
      new { Id = SeedIds.Projector, Name = "Projector", Price = 500m },
      new { Id = SeedIds.Wifi, Name = "Wi-Fi", Price = 300m },
      new { Id = SeedIds.Sound, Name = "Sound", Price = 700m });

    modelBuilder.Entity<HallService>().HasData(
      new { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), HallId = SeedIds.HallA, ServiceId = SeedIds.Projector, Price = 500m },
      new { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), HallId = SeedIds.HallA, ServiceId = SeedIds.Wifi, Price = 300m },
      new { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), HallId = SeedIds.HallB, ServiceId = SeedIds.Projector, Price = 500m },
      new { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), HallId = SeedIds.HallB, ServiceId = SeedIds.Wifi, Price = 300m },
      new { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), HallId = SeedIds.HallB, ServiceId = SeedIds.Sound, Price = 700m },
      new { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), HallId = SeedIds.HallC, ServiceId = SeedIds.Wifi, Price = 300m });
  }
}
