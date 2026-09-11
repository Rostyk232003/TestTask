// PROMPT v2.1: CQRS query бізнес-звіту

using ConferenceRoomBooking.Application.DTOs;
using ConferenceRoomBooking.Application.Interfaces;
using ConferenceRoomBooking.Application.Services.Reports;
using ConferenceRoomBooking.Domain.Interfaces;
using MediatR;

namespace ConferenceRoomBooking.Application.Features.Reports;

/// <summary>
/// Запит на побудову стандартного бізнес-звіту.
/// </summary>
public record GetBusinessReportQuery(DateTime StartDate, DateTime EndDate) : IRequest<BusinessReportDto>;

/// <summary>
/// Обробник запиту бізнес-звіту.
/// </summary>
public class GetBusinessReportQueryHandler : IRequestHandler<GetBusinessReportQuery, BusinessReportDto>
{
  private readonly IAnalyticsRepository _analyticsRepository;
  private readonly ReportDirector _reportDirector;

  public GetBusinessReportQueryHandler(IAnalyticsRepository analyticsRepository, ReportDirector reportDirector)
  {
    this._analyticsRepository = analyticsRepository;
    this._reportDirector = reportDirector;
  }

  public async Task<BusinessReportDto> Handle(GetBusinessReportQuery query, CancellationToken cancellationToken)
  {
    AnalyticsData data = await this._analyticsRepository.GetAsync(query.StartDate, query.EndDate, cancellationToken);
    return this._reportDirector.BuildStandardReport(query.StartDate, query.EndDate, data.Bookings, data.Halls);
  }
}
