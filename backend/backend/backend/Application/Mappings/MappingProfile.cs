using AutoMapper;
using backend.Application.DTOs;
using backend.Domain.Entities;

namespace backend.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MaintenanceRequest, MaintenanceRequestDto>();
        CreateMap<CreateMaintenanceRequestDto, MaintenanceRequest>();
        CreateMap<UpdateMaintenanceRequestDto, MaintenanceRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
