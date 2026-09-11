// PROMPT v2.0: MediatR validation behavior

using FluentValidation;
using MediatR;

namespace ConferenceRoomBooking.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : notnull
{
  private readonly IEnumerable<IValidator<TRequest>> _validators;

  public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
  {
    this._validators = validators;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    if (this._validators.Any())
    {
      ValidationContext<TRequest> context = new ValidationContext<TRequest>(request);
      FluentValidation.Results.ValidationResult[] results = await Task.WhenAll(this._validators.Select(x => x.ValidateAsync(context, cancellationToken)));
      List<FluentValidation.Results.ValidationFailure> failures = results.SelectMany(x => x.Errors).ToList();
      if (failures.Count > 0)
      {
        throw new ValidationException(failures);
      }
    }

    return await next();
  }
}
