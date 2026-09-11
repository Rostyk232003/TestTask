// PROMPT v2.1: DTO бізнес-звіту та аналітики

namespace ConferenceRoomBooking.Application.DTOs;

/// <summary>
/// Зведений бізнес-звіт за заданий період.
/// </summary>
public class BusinessReportDto
{
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public decimal TotalRevenue { get; set; }
  public int TotalBookings { get; set; }
  public decimal OccupancyRatePercentage { get; set; }
  public string? MostPopularHallName { get; set; }
  public List<HallOccupancyDto> HallOccupancy { get; set; } = new List<HallOccupancyDto>();
  public List<ServicePopularityDto> ServicePopularity { get; set; } = new List<ServicePopularityDto>();
  public Dictionary<string, int> WorkingHoursSlotStats { get; set; } = new Dictionary<string, int>();
}

/// <summary>
/// Показники завантаженості окремого залу.
/// </summary>
public class HallOccupancyDto
{
  public Guid HallId { get; set; }
  public string HallName { get; set; } = string.Empty;
  public decimal BookedHours { get; set; }
  public decimal AvailableHours { get; set; }
  public decimal OccupancyRatePercentage { get; set; }
}

/// <summary>
/// Показники використання додаткової послуги.
/// </summary>
public class ServicePopularityDto
{
  public Guid ServiceId { get; set; }
  public string ServiceName { get; set; } = string.Empty;
  public int UsageCount { get; set; }
  public decimal Revenue { get; set; }
}
