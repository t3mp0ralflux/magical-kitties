
using System.Net.Sockets;
using FluentValidation;
using MagicalKitties.Contracts.Responses.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Serilog;

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
        ProblemDetails? problemDetails = null;
        
        switch (exception)
        {
            case ValidationException validationException:
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
            case PostgresException postgresException:
                if (postgresException.Message.Contains("duplicate"))
                {
                    httpContext.Response.StatusCode = 400;
                    ValidationFailureResponse result = new ()
                                                       {
                                                           Errors = [new ValidationResponse()
                                                                     {
                                                                         PropertyName = "Id",
                                                                         Message = "Item already exists"
                                                                     }]
                                                       };

                    await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);
                    return true;
                }
                break;
            case TimeoutException:
            case SocketException:
                problemDetails = new ProblemDetails
                                 {
                                     Status = StatusCodes.Status408RequestTimeout,
                                     Title = "Request Timeout",
                                     Detail = "Failed to process request in time. Please try again.",
                                     Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Status/408"
                                 };

                httpContext.Response.StatusCode = StatusCodes.Status408RequestTimeout;
                        
                Log.Logger.Error(exception, "DB timed out during request. Message: {Message}", exception.Message);
                break;
            default:
                problemDetails = new ProblemDetails
                                 {
                                     Status = StatusCodes.Status500InternalServerError,
                                     Title = "Internal Server Error",
                                     Detail = "A server error has occurred.",
                                     Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Status/500"
                                 };
                Log.Logger.Error(exception, "Uncaught DB error detected. Message: {Message}", exception.Message);

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                
                _logger.LogError(exception, "Uncaught Exception Occurred: {Message}", exception.Message);
                break;
        }

        if (problemDetails is null)
        {
            return false; // no idea what the error is, do some other handling.
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;

    }
}