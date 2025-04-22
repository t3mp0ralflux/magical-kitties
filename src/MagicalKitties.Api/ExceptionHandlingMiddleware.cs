using Microsoft.AspNetCore.Mvc;

namespace MagicalKitties.Api;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);

            ProblemDetails problemDetails = new ProblemDetails
                                            {
                                                Status = StatusCodes.Status500InternalServerError,
                                                Title = "Server Error",
                                                Type = "asdf"
                                            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}