// PROMPT v2.2: Ініціалізація схеми через EF Core migrations

using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Infrastructure.Persistence;

/// <summary>
/// Застосовує міграції для нової БД і підтримує стару EnsureCreated-схему локальної БД.
/// </summary>
public class DatabaseInitializer
{
  private readonly AppDbContext _context;

  public DatabaseInitializer(AppDbContext context)
  {
    this._context = context;
  }

  public async Task InitializeAsync(CancellationToken cancellationToken = default)
  {
    bool hasLegacySchema = await this.HasTableAsync("Halls", cancellationToken)
      && !await this.HasTableAsync("__EFMigrationsHistory", cancellationToken);

    if (hasLegacySchema)
    {
      await this._context.Database.EnsureCreatedAsync(cancellationToken);
      return;
    }

    await this._context.Database.MigrateAsync(cancellationToken);
  }

  private async Task<bool> HasTableAsync(string tableName, CancellationToken cancellationToken)
  {
    return await this._context.Database.SqlQueryRaw<int>(
      "SELECT COUNT(*) AS Value FROM sqlite_master WHERE type = 'table' AND name = {0}", tableName)
      .AnyAsync(x => x > 0, cancellationToken);
  }
}