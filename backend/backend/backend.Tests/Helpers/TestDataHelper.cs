using backend.Application.DTOs;
using backend.Domain.Entities;
using backend.Domain.Enums;

namespace backend.Tests.Helpers
{
    public static class TestDataHelper
    {
        public static MaintenanceRequest CreateMaintenanceRequest(
            int id = 1,
            string eventName = "Test Event",
            string propertyName = "Test Property",
            string description = "Test Description",
            MaintenanceStatus status = MaintenanceStatus.New,
            string createdBy = "test@example.com",
            DateTime? createdDate = null,
            string? updatedBy = null,
            DateTime? updatedDate = null,
            string? imageFileName = null,
            string? imageData = null)
        {
            return new MaintenanceRequest
            {
                Id = id,
                MaintenanceEventName = eventName,
                PropertyName = propertyName,
                Description = description,
                Status = status,
                CreatedBy = createdBy,
                CreatedDate = createdDate ?? DateTime.UtcNow,
                UpdatedBy = updatedBy,
                UpdatedDate = updatedDate,
                ImageFileName = imageFileName,
                ImageData = imageData
            };
        }

        public static CreateMaintenanceRequestDto CreateMaintenanceRequestDto(
            string eventName = "Test Event",
            string propertyName = "Test Property", 
            string description = "Test Description",
            string createdBy = "test@example.com",
            string? imageFileName = null,
            string? imageData = null)
        {
            return new CreateMaintenanceRequestDto
            {
                MaintenanceEventName = eventName,
                PropertyName = propertyName,
                Description = description,
                CreatedBy = createdBy,
                ImageFileName = imageFileName,
                ImageData = imageData
            };
        }

        public static UpdateMaintenanceRequestDto CreateUpdateMaintenanceRequestDto(
            string eventName = "Updated Event",
            string propertyName = "Updated Property",
            string description = "Updated Description", 
            string updatedBy = "test@example.com",
            MaintenanceStatus? status = null,
            string? imageFileName = null,
            string? imageData = null)
        {
            return new UpdateMaintenanceRequestDto
            {
                MaintenanceEventName = eventName,
                PropertyName = propertyName,
                Description = description,
                Status = status,
                UpdatedBy = updatedBy,
                ImageFileName = imageFileName,
                ImageData = imageData
            };
        }

        public static MaintenanceRequestDto CreateMaintenanceRequestResponseDto(
            int id = 1,
            string eventName = "Test Event",
            string propertyName = "Test Property",
            string description = "Test Description",
            MaintenanceStatus status = MaintenanceStatus.New,
            string createdBy = "test@example.com",
            DateTime? createdDate = null,
            string? updatedBy = null,
            DateTime? updatedDate = null,
            string? imageFileName = null,
            string? imageData = null)
        {
            return new MaintenanceRequestDto
            {
                Id = id,
                MaintenanceEventName = eventName,
                PropertyName = propertyName,
                Description = description,
                Status = status,
                CreatedBy = createdBy,
                CreatedDate = createdDate ?? DateTime.UtcNow,
                UpdatedBy = updatedBy,
                UpdatedDate = updatedDate,
                ImageFileName = imageFileName,
                ImageData = imageData
            };
        }

        public static List<MaintenanceRequest> CreateMaintenanceRequestList(int count = 3)
        {
            var requests = new List<MaintenanceRequest>();
            for (int i = 1; i <= count; i++)
            {
                requests.Add(CreateMaintenanceRequest(
                    id: i,
                    eventName: $"Event {i}",
                    propertyName: $"Property {i}",
                    description: $"Description {i}",
                    status: (MaintenanceStatus)(i % 3), // Cycle through statuses
                    createdBy: $"user{i}@example.com"
                ));
            }
            return requests;
        }

        public static List<MaintenanceRequestDto> CreateMaintenanceRequestDtoList(int count = 3)
        {
            var requests = new List<MaintenanceRequestDto>();
            for (int i = 1; i <= count; i++)
            {
                requests.Add(CreateMaintenanceRequestResponseDto(
                    id: i,
                    eventName: $"Event {i}",
                    propertyName: $"Property {i}",
                    description: $"Description {i}",
                    status: (MaintenanceStatus)(i % 3), // Cycle through statuses
                    createdBy: $"user{i}@example.com"
                ));
            }
            return requests;
        }
    }
}