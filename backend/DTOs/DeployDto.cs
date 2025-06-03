namespace SmartNameplate.Api.DTOs;

public class DeployHistoryDto
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public int CardId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? DeployedAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string? DeployedBy { get; set; }
    public bool IsScheduled { get; set; }
    public int RetryCount { get; set; }
    
    // Navigation properties
    public DeviceDto? Device { get; set; }
    public CardResponseDto? Card { get; set; }
}

public class DeployRequestDto
{
    public int[] DeviceIds { get; set; } = Array.Empty<int>();
    public int CardId { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public string? DeployedBy { get; set; }
}

public class DeployResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TotalDevices { get; set; }
    public int SuccessfulDeploys { get; set; }
    public int FailedDeploys { get; set; }
    public List<DeployHistoryDto> Results { get; set; } = new();
}
