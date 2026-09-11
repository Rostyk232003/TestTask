// PROMPT v2.0: FluentValidation validators для CQRS

using ConferenceRoomBooking.Application.Features.Bookings;
using ConferenceRoomBooking.Application.Features.Halls;
using FluentValidation;

namespace ConferenceRoomBooking.Application.Features;

public class CreateHallCommandValidator : AbstractValidator<CreateHallCommand>
{
  public CreateHallCommandValidator()
  {
    this.RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    this.RuleFor(x => x.Capacity).GreaterThan(0);
    this.RuleFor(x => x.HourlyRate).GreaterThan(0m);
    this.RuleFor(x => x.ServiceIds).Must(x => x.Distinct().Count() == x.Count).WithMessage("ServiceIds must be unique.");
  }
}

public class UpdateHallCommandValidator : AbstractValidator<UpdateHallCommand>
{
  public UpdateHallCommandValidator()
  {
    this.RuleFor(x => x.Id).NotEmpty();
    this.RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    this.RuleFor(x => x.Capacity).GreaterThan(0);
    this.RuleFor(x => x.HourlyRate).GreaterThan(0m);
    this.RuleFor(x => x.ServiceIds).Must(x => x.Distinct().Count() == x.Count).WithMessage("ServiceIds must be unique.");
  }
}

public class GetAvailableHallsQueryValidator : AbstractValidator<GetAvailableHallsQuery>
{
  public GetAvailableHallsQueryValidator()
  {
    this.RuleFor(x => x.StartTime).LessThan(x => x.EndTime);
    this.RuleFor(x => x.Capacity).GreaterThan(0);
  }
}

public class BookHallCommandValidator : AbstractValidator<BookHallCommand>
{
  public BookHallCommandValidator()
  {
    this.RuleFor(x => x.HallId).NotEmpty();
    this.RuleFor(x => x.DurationHours).InclusiveBetween(1, 24);
    this.RuleFor(x => x.ServiceIds).Must(x => x.Distinct().Count() == x.Count).WithMessage("ServiceIds must be unique.");
  }
}
