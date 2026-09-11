// PROMPT v2.1: Реалізація Builder бізнес-звіту

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Application.Services.Reports;

/// <summary>
/// Формує бізнес-звіт без залежності від бази даних.
/// </summary>
public class BusinessReportBuilder : IReportBuilder
{
  private const int WorkingDayStartHour = 6;
  private const int WorkingDayEndHour = 23;
  private BusinessReportDto _report = new BusinessReportDto();

  public IReportBuilder Reset()
  {
    this._report = new BusinessReportDto();
    return this;
  }

  public IReportBuilder SetTimePeriod(DateTime startDate, DateTime endDate)
  {
    this._report.StartDate = startDate;
    this._report.EndDate = endDate;
    return this;
  }

  public IReportBuilder CalculateRevenue(List<Booking> bookings)
  {
    List<Booking> activeBookings = this.GetActiveBookings(bookings);
    this._report.TotalRevenue = activeBookings.Sum(x => x.TotalPrice);
    this._report.TotalBookings = activeBookings.Count;
    return this;
  }

  public IReportBuilder CalculateOccupancy(List<Booking> bookings, List<Hall> halls)
  {
    decimal totalBookedHours = 0m;
    decimal totalAvailableHours = 0m;

    foreach (Hall hall in halls.Where(x => x.IsActive).OrderBy(x => x.Name))
    {
      decimal availableHours = this.CalculateAvailableHours(this._report.StartDate, this._report.EndDate);
      decimal bookedHours = bookings
        .Where(x => x.HallId == hall.Id && x.Status != BookingStatus.Cancelled)
        .Sum(x => this.CalculateWorkingHours(x.StartsAt, x.EndsAt, this._report.StartDate, this._report.EndDate));
      decimal occupancy = availableHours == 0m ? 0m : decimal.Round(bookedHours / availableHours * 100m, 2);

      this._report.HallOccupancy.Add(new HallOccupancyDto
      {
        HallId = hall.Id,
        HallName = hall.Name,
        BookedHours = decimal.Round(bookedHours, 2),
        AvailableHours = decimal.Round(availableHours, 2),
        OccupancyRatePercentage = occupancy,
      });

      totalBookedHours += bookedHours;
      totalAvailableHours += availableHours;
    }

    this._report.OccupancyRatePercentage = totalAvailableHours == 0m
      ? 0m
      : decimal.Round(totalBookedHours / totalAvailableHours * 100m, 2);

    this.SetWorkingHoursSlotStats(bookings);
    return this;
  }

  public IReportBuilder SetMostPopularHall(List<Booking> bookings, List<Hall> halls)
  {
    Dictionary<Guid, int> counts = this.GetActiveBookings(bookings)
      .GroupBy(x => x.HallId)
      .ToDictionary(x => x.Key, x => x.Count());

    this._report.MostPopularHallName = halls
      .Where(x => counts.ContainsKey(x.Id))
      .OrderByDescending(x => counts[x.Id])
      .ThenBy(x => x.Name)
      .Select(x => x.Name)
      .FirstOrDefault();

    return this;
  }

  public IReportBuilder SetMostPopularServices(List<Booking> bookings)
  {
    this._report.ServicePopularity = this.GetActiveBookings(bookings)
      .SelectMany(x => x.BookingServices)
      .GroupBy(x => new { x.ServiceId, x.ServiceName })
      .Select(x => new ServicePopularityDto
      {
        ServiceId = x.Key.ServiceId,
        ServiceName = x.Key.ServiceName,
        UsageCount = x.Count(),
        Revenue = x.Sum(y => y.UnitPrice),
      })
      .OrderByDescending(x => x.UsageCount)
      .ThenBy(x => x.ServiceName)
      .ToList();

    return this;
  }

  public BusinessReportDto Build()
  {
    return new BusinessReportDto
    {
      StartDate = this._report.StartDate,
      EndDate = this._report.EndDate,
      TotalRevenue = this._report.TotalRevenue,
      TotalBookings = this._report.TotalBookings,
      OccupancyRatePercentage = this._report.OccupancyRatePercentage,
      MostPopularHallName = this._report.MostPopularHallName,
      HallOccupancy = this._report.HallOccupancy.ToList(),
      ServicePopularity = this._report.ServicePopularity.ToList(),
      WorkingHoursSlotStats = this._report.WorkingHoursSlotStats.ToDictionary(x => x.Key, x => x.Value),
    };
  }

  private List<Booking> GetActiveBookings(List<Booking> bookings)
  {
    return bookings.Where(x => x.Status != BookingStatus.Cancelled).ToList();
  }

  private decimal CalculateAvailableHours(DateTime startDate, DateTime endDate)
  {
    decimal total = 0m;
    DateTime currentDate = startDate.Date;
    while (currentDate < endDate)
    {
      total += (decimal)this.GetOverlapHours(currentDate.AddHours(WorkingDayStartHour), currentDate.AddHours(WorkingDayEndHour), startDate, endDate);
      currentDate = currentDate.AddDays(1);
    }

    return total;
  }

  private decimal CalculateWorkingHours(DateTime bookingStart, DateTime bookingEnd, DateTime periodStart, DateTime periodEnd)
  {
    DateTime clippedStart = bookingStart > periodStart ? bookingStart : periodStart;
    DateTime clippedEnd = bookingEnd < periodEnd ? bookingEnd : periodEnd;
    if (clippedStart >= clippedEnd)
    {
      return 0m;
    }

    decimal total = 0m;
    DateTime currentDate = clippedStart.Date;
    while (currentDate <= clippedEnd.Date)
    {
      total += (decimal)this.GetOverlapHours(currentDate.AddHours(WorkingDayStartHour), currentDate.AddHours(WorkingDayEndHour), clippedStart, clippedEnd);
      currentDate = currentDate.AddDays(1);
    }

    return total;
  }

  private double GetOverlapHours(DateTime rangeStart, DateTime rangeEnd, DateTime targetStart, DateTime targetEnd)
  {
    DateTime start = rangeStart > targetStart ? rangeStart : targetStart;
    DateTime end = rangeEnd < targetEnd ? rangeEnd : targetEnd;
    return start >= end ? 0d : (end - start).TotalHours;
  }

  private void SetWorkingHoursSlotStats(List<Booking> bookings)
  {
    Dictionary<string, int> stats = new Dictionary<string, int>();
    foreach (Booking booking in this.GetActiveBookings(bookings))
    {
      DateTime cursor = booking.StartsAt;
      while (cursor < booking.EndsAt)
      {
        string slot = this.GetSlotName(cursor.Hour);
        if (!stats.ContainsKey(slot))
        {
          stats[slot] = 0;
        }

        stats[slot]++;
        cursor = cursor.AddHours(1);
      }
    }

    this._report.WorkingHoursSlotStats = stats;
  }

  private string GetSlotName(int hour)
  {
    if (hour >= 6 && hour < 9) return "06:00-09:00";
    if (hour >= 12 && hour < 14) return "12:00-14:00";
    if (hour >= 18 && hour < 23) return "18:00-23:00";
    if (hour >= 9 && hour < 18) return "09:00-18:00";
    return "OutsideWorkingHours";
  }
}
