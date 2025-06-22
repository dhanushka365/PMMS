using backend.Domain.Entities;
using backend.Domain.Enums;

namespace backend.Application.Interfaces;

public interface IMaintenanceRequestRepository
{
    Task<IEnumerable<MaintenanceRequest>> GetAllAsync();
    Task<MaintenanceRequest?> GetByIdAsync(int id);
    Task<IEnumerable<MaintenanceRequest>> GetByStatusAsync(MaintenanceStatus status);
    Task<IEnumerable<MaintenanceRequest>> SearchAsync(string searchTerm);
    Task<MaintenanceRequest> CreateAsync(MaintenanceRequest request);
    Task<MaintenanceRequest> UpdateAsync(MaintenanceRequest request);
    Task<bool> DeleteAsync(int id);
}
