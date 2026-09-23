namespace SupplyFlow.Application.DTOs.Common;

public class ApiErrorResponseDto
{
    public bool Success { get; set; } = false;

    public int StatusCode { get; set; }

    public string Message { get; set; }
        = string.Empty;

    public object? Errors { get; set; }

    public DateTime Timestamp { get; set; }
        = DateTime.UtcNow;
}