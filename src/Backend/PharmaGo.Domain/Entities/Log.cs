using PharmaGo.Domain.Enums;

namespace PharmaGo.Domain.Entities;

public class Log
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public LogLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? AdditionalData { get; set; }
}