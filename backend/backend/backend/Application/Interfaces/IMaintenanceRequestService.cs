using backend.Application.DTOs;
using backend.Domain.Enums;

namespace backend.Application.Interfaces;

public interface IMaintenanceRequestService
{
    Task<IEnumerable<MaintenanceRequestDto>> GetAllMaintenanceRequestsAsync();
    Task<MaintenanceRequestDto?> GetMaintenanceRequestByIdAsync(int id);
    Task<IEnumerable<MaintenanceRequestDto>> GetMaintenanceRequestsByStatusAsync(MaintenanceStatus status);
    Task<IEnumerable<MaintenanceRequestDto>> SearchMaintenanceRequestsAsync(string searchTerm);
    Task<MaintenanceRequestDto> CreateMaintenanceRequestAsync(CreateMaintenanceRequestDto dto);
    Task<MaintenanceRequestDto> UpdateMaintenanceRequestAsync(int id, UpdateMaintenanceRequestDto dto, UserRole userRole);
    Task<bool> DeleteMaintenanceRequestAsync(int id);
}
