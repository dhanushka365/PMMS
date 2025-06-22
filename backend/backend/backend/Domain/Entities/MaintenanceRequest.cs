using backend.Domain.Enums;

namespace backend.Domain.Entities;

public class MaintenanceRequest
{
    public int Id { get; set; }
    public string MaintenanceEventName { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.New;
    public string? ImageFileName { get; set; }
    public string? ImageData { get; set; } 
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}
