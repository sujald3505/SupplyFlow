using SupplyFlow.Application.DTOs.Common;
using System.Net;
using System.Text.Json;

namespace SupplyFlow.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware>
        _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An unhandled exception occurred.");

            await HandleExceptionAsync(
                context,
                exception);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType =
            "application/json";

        var response =
            exception switch
            {
                UnauthorizedAccessException =>
                    new ApiErrorResponseDto
                    {
                        StatusCode =
                            (int)HttpStatusCode.Unauthorized,

                        Message =
                            "You are not authorized to perform this action."
                    },

                KeyNotFoundException =>
                    new ApiErrorResponseDto
                    {
                        StatusCode =
                            (int)HttpStatusCode.NotFound,

                        Message =
                            exception.Message
                    },

                InvalidOperationException =>
                    new ApiErrorResponseDto
                    {
                        StatusCode =
                            (int)HttpStatusCode.BadRequest,

                        Message =
                            exception.Message
                    },

                ArgumentException =>
                    new ApiErrorResponseDto
                    {
                        StatusCode =
                            (int)HttpStatusCode.BadRequest,

                        Message =
                            exception.Message
                    },

                _ =>
                    new ApiErrorResponseDto
                    {
                        StatusCode =
                            (int)HttpStatusCode
                                .InternalServerError,

                        Message =
                            "An unexpected error occurred."
                    }
            };

        context.Response.StatusCode =
            response.StatusCode;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}