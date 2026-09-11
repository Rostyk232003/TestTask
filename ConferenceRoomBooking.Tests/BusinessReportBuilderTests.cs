// PROMPT v2.1: Unit-тести Builder бізнес-звіту

using ConferenceRoomBooking.Application.Services.Reports;
using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Tests;

public class BusinessReportBuilderTests
{
  [Fact]
  public void Build_ShouldCalculateRevenueBookingsAndOccupancy()
  {
    Hall hall = new Hall("Hall A", 50, 2000m);
    Booking booking = new Booking(hall.Id, new DateTime(2026, 9, 1, 10, 0, 0), new DateTime(2026, 9, 1, 12, 0, 0), 4000m);
    BusinessReportBuilder builder = new BusinessReportBuilder();

    ConferenceRoomBooking.Application.DTOs.BusinessReportDto report = builder
      .Reset()
      .SetTimePeriod(new DateTime(2026, 9, 1, 0, 0, 0), new DateTime(2026, 9, 2, 0, 0, 0))
      .CalculateRevenue(new List<Booking> { booking })
      .CalculateOccupancy(new List<Booking> { booking }, new List<Hall> { hall })
      .SetMostPopularHall(new List<Booking> { booking }, new List<Hall> { hall })
      .SetMostPopularServices(new List<Booking> { booking })
      .Build();

    Assert.Equal(4000m, report.TotalRevenue);
    Assert.Equal(1, report.TotalBookings);
    Assert.Equal(2m, report.HallOccupancy[0].BookedHours);
    Assert.Equal(17m, report.HallOccupancy[0].AvailableHours);
    Assert.Equal(11.76m, report.HallOccupancy[0].OccupancyRatePercentage);
    Assert.Equal("Hall A", report.MostPopularHallName);
  }

  [Fact]
  public void Build_ShouldUseBookingServiceSnapshotAndIgnoreCancelledBooking()
  {
    Hall hall = new Hall("Hall A", 50, 2000m);
    Service projector = new Service("Projector", 500m);
    Booking activeBooking = new Booking(hall.Id, new DateTime(2026, 9, 1, 10, 0, 0), new DateTime(2026, 9, 1, 11, 0, 0), 2500m);
    activeBooking.BookingServices.Add(new BookingService(activeBooking.Id, projector.Id, projector.Name, 500m));
    Booking cancelledBooking = new Booking(hall.Id, new DateTime(2026, 9, 1, 12, 0, 0), new DateTime(2026, 9, 1, 13, 0, 0), 9999m);
    cancelledBooking.Cancel();
    BusinessReportBuilder builder = new BusinessReportBuilder();

    ConferenceRoomBooking.Application.DTOs.BusinessReportDto report = builder
      .Reset()
      .SetTimePeriod(new DateTime(2026, 9, 1), new DateTime(2026, 9, 2))
      .CalculateRevenue(new List<Booking> { activeBooking, cancelledBooking })
      .CalculateOccupancy(new List<Booking> { activeBooking, cancelledBooking }, new List<Hall> { hall })
      .SetMostPopularHall(new List<Booking> { activeBooking, cancelledBooking }, new List<Hall> { hall })
      .SetMostPopularServices(new List<Booking> { activeBooking, cancelledBooking })
      .Build();

    Assert.Equal(2500m, report.TotalRevenue);
    Assert.Equal(1, report.TotalBookings);
    Assert.Single(report.ServicePopularity);
    Assert.Equal(500m, report.ServicePopularity[0].Revenue);
  }
}
