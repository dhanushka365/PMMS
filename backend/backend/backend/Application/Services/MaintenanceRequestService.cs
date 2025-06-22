using AutoMapper;
using backend.Application.DTOs;
using backend.Application.Interfaces;
using backend.Domain.Entities;
using backend.Domain.Enums;

namespace backend.Application.Services;

public class MaintenanceRequestService : IMaintenanceRequestService
{
    private readonly IMaintenanceRequestRepository _repository;
    private readonly IMapper _mapper;

    public MaintenanceRequestService(IMaintenanceRequestRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MaintenanceRequestDto>> GetAllMaintenanceRequestsAsync()
    {
        var requests = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<MaintenanceRequestDto>>(requests);
    }

    public async Task<MaintenanceRequestDto?> GetMaintenanceRequestByIdAsync(int id)
    {
        var request = await _repository.GetByIdAsync(id);
        return request == null ? null : _mapper.Map<MaintenanceRequestDto>(request);
    }

    public async Task<IEnumerable<MaintenanceRequestDto>> GetMaintenanceRequestsByStatusAsync(MaintenanceStatus status)
    {
        var requests = await _repository.GetByStatusAsync(status);
        return _mapper.Map<IEnumerable<MaintenanceRequestDto>>(requests);
    }

    public async Task<IEnumerable<MaintenanceRequestDto>> SearchMaintenanceRequestsAsync(string searchTerm)
    {
        var requests = await _repository.SearchAsync(searchTerm);
        return _mapper.Map<IEnumerable<MaintenanceRequestDto>>(requests);
    }

    public async Task<MaintenanceRequestDto> CreateMaintenanceRequestAsync(CreateMaintenanceRequestDto dto)
    {
        var request = _mapper.Map<MaintenanceRequest>(dto);
        request.Status = MaintenanceStatus.New;
        request.CreatedDate = DateTime.UtcNow;
        
        var createdRequest = await _repository.CreateAsync(request);
        return _mapper.Map<MaintenanceRequestDto>(createdRequest);
    }

    public async Task<MaintenanceRequestDto> UpdateMaintenanceRequestAsync(int id, UpdateMaintenanceRequestDto dto, UserRole userRole)
    {
        var existingRequest = await _repository.GetByIdAsync(id);
        if (existingRequest == null)
            throw new ArgumentException($"Maintenance request with ID {id} not found.");

        // Map basic fields that both roles can edit
        existingRequest.MaintenanceEventName = dto.MaintenanceEventName;
        existingRequest.PropertyName = dto.PropertyName;
        existingRequest.Description = dto.Description;
        existingRequest.UpdatedBy = dto.UpdatedBy;
        
        // Only update image if provided
        if (!string.IsNullOrEmpty(dto.ImageFileName))
        {
            existingRequest.ImageFileName = dto.ImageFileName;
            existingRequest.ImageData = dto.ImageData;
        }

        // Only Admin can update status
        if (userRole == UserRole.Admin && dto.Status.HasValue)
        {
            existingRequest.Status = dto.Status.Value;
        }

        var updatedRequest = await _repository.UpdateAsync(existingRequest);
        return _mapper.Map<MaintenanceRequestDto>(updatedRequest);
    }

    public async Task<bool> DeleteMaintenanceRequestAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}
