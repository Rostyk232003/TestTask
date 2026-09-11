// PROMPT v2.0: Глобальна обробка помилок API

using System.Text.Json;
using FluentValidation;
using ConferenceRoomBooking.Application.Exceptions;

namespace ConferenceRoomBooking.API.Middleware;

public class ExceptionHandlingMiddleware
{
  private readonly RequestDelegate _next;

  public ExceptionHandlingMiddleware(RequestDelegate next)
  {
    this._next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await this._next(context);
    }
    catch (ValidationException exception)
    {
      await this.WriteErrorAsync(context, StatusCodes.Status400BadRequest, exception.Errors.Select(x => x.ErrorMessage).ToList());
    }
    catch (NotFoundException exception)
    {
      await this.WriteErrorAsync(context, StatusCodes.Status404NotFound, new List<string> { exception.Message });
    }
    catch (ConflictException exception)
    {
      await this.WriteErrorAsync(context, StatusCodes.Status409Conflict, new List<string> { exception.Message });
    }
    catch (UnprocessableEntityException exception)
    {
      await this.WriteErrorAsync(context, StatusCodes.Status422UnprocessableEntity, new List<string> { exception.Message });
    }
  }

  private async Task WriteErrorAsync(HttpContext context, int statusCode, List<string> errors)
  {
    context.Response.StatusCode = statusCode;
    context.Response.ContentType = "application/json";
    string body = JsonSerializer.Serialize(new { statusCode, errors });
    await context.Response.WriteAsync(body);
  }
}