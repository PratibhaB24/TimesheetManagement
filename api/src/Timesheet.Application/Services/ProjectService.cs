using AutoMapper;
using Timesheet.Application.DTOs.Project;
using Timesheet.Application.Interfaces.Repositories;
using Timesheet.Application.Interfaces.Services;
using Timesheet.Domain.Entities;
using Timesheet.Domain.Enums;

namespace Timesheet.Application.Services
{
    /// <summary>
    /// Project Service Implementation.
    /// Handles CRUD operations for projects (Manager role only).
    /// </summary>
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProjectDto?> GetByIdAsync(int id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            return project != null ? _mapper.Map<ProjectDto>(project) : null;
        }

        public async Task<ProjectDto?> GetByCodeAsync(string code)
        {
            var project = await _unitOfWork.Projects.GetByCodeAsync(code);
            return project != null ? _mapper.Map<ProjectDto>(project) : null;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync()
        {
            var projects = await _unitOfWork.Projects.GetAllAsync();
            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<IEnumerable<ProjectDto>> GetActiveProjectsAsync()
        {
            var projects = await _unitOfWork.Projects.GetActiveProjectsAsync();
            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
        {
            // Validate unique project code
            if (await _unitOfWork.Projects.CodeExistsAsync(dto.Code))
            {
                throw new InvalidOperationException($"Project with code '{dto.Code}' already exists.");
            }

            var project = _mapper.Map<Project>(dto);
            project.Status = ProjectStatus.Active;

            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProjectDto>(project);
        }

        public async Task<ProjectDto?> UpdateAsync(int id, UpdateProjectDto dto)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project == null) return null;

            _mapper.Map(dto, project);
            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProjectDto>(project);
        }

        public async Task<bool> ActivateAsync(int id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project == null) return false;

            project.Status = ProjectStatus.Active;
            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project == null) return false;

            project.Status = ProjectStatus.Inactive;
            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
