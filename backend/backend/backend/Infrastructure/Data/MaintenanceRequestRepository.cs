using Microsoft.EntityFrameworkCore;
using backend.Application.Interfaces;
using backend.Domain.Entities;
using backend.Domain.Enums;
using backend.Infrastructure.Data;

namespace backend.Infrastructure.Repositories;

public class MaintenanceRequestRepository : IMaintenanceRequestRepository
{
    private readonly ApplicationDbContext _context;

    public MaintenanceRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MaintenanceRequest>> GetAllAsync()
    {
        return await _context.MaintenanceRequests
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<MaintenanceRequest?> GetByIdAsync(int id)
    {
        return await _context.MaintenanceRequests.FindAsync(id);
    }

    public async Task<IEnumerable<MaintenanceRequest>> GetByStatusAsync(MaintenanceStatus status)
    {
        return await _context.MaintenanceRequests
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceRequest>> SearchAsync(string searchTerm)
    {
        return await _context.MaintenanceRequests
            .Where(x => x.MaintenanceEventName.Contains(searchTerm) ||
                       x.PropertyName.Contains(searchTerm) ||
                       x.Description.Contains(searchTerm))
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<MaintenanceRequest> CreateAsync(MaintenanceRequest request)
    {
        _context.MaintenanceRequests.Add(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task<MaintenanceRequest> UpdateAsync(MaintenanceRequest request)
    {
        request.UpdatedDate = DateTime.UtcNow;
        _context.MaintenanceRequests.Update(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var request = await _context.MaintenanceRequests.FindAsync(id);
        if (request == null)
            return false;

        _context.MaintenanceRequests.Remove(request);
        await _context.SaveChangesAsync();
        return true;
    }
}
