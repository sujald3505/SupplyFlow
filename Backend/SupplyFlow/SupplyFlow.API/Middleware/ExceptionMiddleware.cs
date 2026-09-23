using SupplyFlow.Application.DTOs.Common;
using System.Net;

namespace SupplyFlow.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unhandled exception occurred.");

            await HandleExceptionAsync(
                context,
                ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType =
            "application/json";

        var response = exception switch
        {
            UnauthorizedAccessException =>
                new ErrorResponse
                {
                    StatusCode =
                        (int)HttpStatusCode.Unauthorized,

                    Message =
                        exception.Message
                },

            KeyNotFoundException =>
                new ErrorResponse
                {
                    StatusCode =
                        (int)HttpStatusCode.NotFound,

                    Message =
                        exception.Message
                },

            InvalidOperationException =>
                new ErrorResponse
                {
                    StatusCode =
                        (int)HttpStatusCode.BadRequest,

                    Message =
                        exception.Message
                },

            _ =>
                new ErrorResponse
                {
                    StatusCode =
                        (int)
                        HttpStatusCode.InternalServerError,

                    Message =
                        "An unexpected error occurred."
                }
        };

        context.Response.StatusCode =
            response.StatusCode;

        await context.Response.WriteAsJsonAsync(
            response);
    }
}