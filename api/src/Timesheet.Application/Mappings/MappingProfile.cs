using AutoMapper;
using Timesheet.Application.DTOs.User;
using Timesheet.Application.DTOs.Project;
using Timesheet.Application.DTOs.ProjectAssignment;
using Timesheet.Application.DTOs.Timesheet;
using Timesheet.Domain.Entities;

namespace Timesheet.Application.Mappings
{
    /// <summary>
    /// AutoMapper Profile for mapping between Entities and DTOs.
    /// 
    /// WHY AUTOMAPPER?
    /// - Reduces boilerplate code for object-to-object mapping
    /// - Centralizes mapping logic
    /// - Handles nested object mapping automatically
    /// 
    /// HOW IT WORKS:
    /// - CreateMap<Source, Destination>() defines a mapping
    /// - ForMember() allows custom property mapping
    /// - ReverseMap() creates bi-directional mapping
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ============ USER MAPPINGS ============
            CreateMap<User, UserDto>();
            
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Password is hashed in service
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());

            // ============ PROJECT MAPPINGS ============
            CreateMap<Project, ProjectDto>();

            CreateMap<CreateProjectDto, Project>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.ProjectStatus.Active))
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());

            CreateMap<UpdateProjectDto, Project>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Code, opt => opt.Ignore()) // Code cannot be updated
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());

            // ============ PROJECT ASSIGNMENT MAPPINGS ============
            CreateMap<ProjectAssignment, ProjectAssignmentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.ProjectCode, opt => opt.MapFrom(src => src.Project != null ? src.Project.Code : string.Empty))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));

            CreateMap<CreateProjectAssignmentDto, ProjectAssignment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());

            // ============ TIMESHEET MAPPINGS ============
            CreateMap<Domain.Entities.Timesheet, TimesheetDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.TotalHours, opt => opt.MapFrom(src => src.Entries.Sum(e => e.Hours)));

            CreateMap<CreateTimesheetDto, Domain.Entities.Timesheet>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Set from authenticated user
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.TimesheetStatus.Draft))
                .ForMember(dest => dest.RejectionComments, opt => opt.Ignore())
                .ForMember(dest => dest.Entries, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());

            // ============ TIMESHEET ENTRY MAPPINGS ============
            CreateMap<TimesheetEntry, TimesheetEntryDto>()
                .ForMember(dest => dest.ProjectCode, opt => opt.MapFrom(src => src.Project != null ? src.Project.Code : string.Empty))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));

            CreateMap<CreateTimesheetEntryDto, TimesheetEntry>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TimesheetId, opt => opt.Ignore())
                .ForMember(dest => dest.Timesheet, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());
        }
    }
}
