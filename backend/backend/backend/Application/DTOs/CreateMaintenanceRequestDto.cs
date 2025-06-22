namespace backend.Application.DTOs
{
    public class CreateMaintenanceRequestDto
    {
        public string MaintenanceEventName { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageFileName { get; set; }
        public string? ImageData { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
