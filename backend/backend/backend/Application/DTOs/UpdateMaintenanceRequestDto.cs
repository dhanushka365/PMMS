using backend.Domain.Enums;

namespace backend.Application.DTOs
{
    public class UpdateMaintenanceRequestDto
    {
        public string MaintenanceEventName { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public MaintenanceStatus? Status { get; set; }
        public string? ImageFileName { get; set; }
        public string? ImageData { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
