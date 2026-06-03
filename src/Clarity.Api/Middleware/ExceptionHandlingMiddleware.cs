using System.Text.Json;
using Clarity.Application.Common.Exceptions;

namespace Clarity.Api.Middleware;

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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? "unknown";

        var (statusCode, response) = exception switch
        {
            ValidationException validationEx => (StatusCodes.Status400BadRequest, new ErrorResponse
            {
                Type = "ValidationError",
                Title = "One or more validation errors occurred.",
                Status = 400,
                CorrelationId = correlationId,
                Errors = validationEx.Errors
            }),
            NotFoundException notFoundEx => (StatusCodes.Status404NotFound, new ErrorResponse
            {
                Type = "NotFound",
                Title = notFoundEx.Message,
                Status = 404,
                CorrelationId = correlationId
            }),
            ForbiddenException forbiddenEx => (StatusCodes.Status403Forbidden, new ErrorResponse
            {
                Type = "Forbidden",
                Title = forbiddenEx.Message,
                Status = 403,
                CorrelationId = correlationId
            }),
            _ => (StatusCodes.Status500InternalServerError, new ErrorResponse
            {
                Type = "InternalError",
                Title = "An unexpected error occurred. Please contact support.",
                Status = 500,
                CorrelationId = correlationId
            })
        };

        if (statusCode == 500)
        {
            _logger.LogError(exception, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);
        }
        else
        {
            _logger.LogWarning("Handled exception: {ExceptionType} - {Message}. CorrelationId: {CorrelationId}",
                exception.GetType().Name, exception.Message, correlationId);
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}

public class ErrorResponse
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; set; }
}
