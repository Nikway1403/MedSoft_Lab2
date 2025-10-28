using System.Net;
using System.Text.Json;

namespace Server.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/fhir+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var outcome = new
        {
            resourceType = "OperationOutcome",
            issue = new[]
            {
                new
                {
                    severity = "error",
                    code = "exception",
                    diagnostics = ex.Message
                }
            }
        };

        var json = JsonSerializer.Serialize(outcome, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
}