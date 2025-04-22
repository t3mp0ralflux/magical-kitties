using FluentValidation;
using MagicalKitties.Contracts.Responses.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MagicalKitties.Api;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            httpContext.Response.StatusCode = 400;
            ValidationFailureResponse validationFailureResponse = new()
                                                                  {
                                                                      Errors = validationException.Errors.Select(x => new ValidationResponse
                                                                                                                      {
                                                                                                                          PropertyName = x.PropertyName,
                                                                                                                          Message = x.ErrorMessage
                                                                                                                      })
                                                                  };

            await httpContext.Response.WriteAsJsonAsync(validationFailureResponse, cancellationToken);

            return true;
        }
        
        _logger.LogError(exception, "Exception Occurred: {Message}", exception.Message);

        ProblemDetails problemDetails = new ProblemDetails
                                        {
                                            Status = StatusCodes.Status500InternalServerError,
                                            Title = "Server Error",
                                            Type = "Asdf"
                                        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}