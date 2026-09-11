// PROMPT v2.1: Валідація query бізнес-звіту

using FluentValidation;

namespace ConferenceRoomBooking.Application.Features.Reports;

/// <summary>
/// Перевіряє часовий період аналітичного звіту.
/// </summary>
public class GetBusinessReportValidator : AbstractValidator<GetBusinessReportQuery>
{
  public GetBusinessReportValidator()
  {
    this.RuleFor(x => x.StartDate).LessThan(x => x.EndDate);
    this.RuleFor(x => x.EndDate - x.StartDate)
      .LessThanOrEqualTo(TimeSpan.FromDays(366))
      .WithMessage("Report period cannot exceed 366 days.");
  }
}
