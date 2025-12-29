using AutoMapper;
using Timesheet.Application.DTOs.ProjectAssignment;
using Timesheet.Application.Interfaces.Repositories;
using Timesheet.Application.Interfaces.Services;
using Timesheet.Domain.Entities;

namespace Timesheet.Application.Services
{
    /// <summary>
    /// ProjectAssignment Service Implementation.
    /// Handles project assignments to employees (Manager role only).
    /// </summary>
    public class ProjectAssignmentService : IProjectAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProjectAssignmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProjectAssignmentDto?> GetByIdAsync(int id)
        {
            var assignment = await _unitOfWork.ProjectAssignments.GetByIdAsync(id);
            return assignment != null ? _mapper.Map<ProjectAssignmentDto>(assignment) : null;
        }

        public async Task<IEnumerable<ProjectAssignmentDto>> GetAllAsync()
        {
            var assignments = await _unitOfWork.ProjectAssignments.GetAllAsync();
            return _mapper.Map<IEnumerable<ProjectAssignmentDto>>(assignments);
        }

        public async Task<IEnumerable<ProjectAssignmentDto>> GetUserAssignmentsAsync(int userId)
        {
            var assignments = await _unitOfWork.ProjectAssignments.GetUserAssignmentsAsync(userId);
            return _mapper.Map<IEnumerable<ProjectAssignmentDto>>(assignments);
        }

        public async Task<IEnumerable<ProjectAssignmentDto>> GetProjectAssignmentsAsync(int projectId)
        {
            var assignments = await _unitOfWork.ProjectAssignments.GetProjectAssignmentsAsync(projectId);
            return _mapper.Map<IEnumerable<ProjectAssignmentDto>>(assignments);
        }

        public async Task<ProjectAssignmentDto> CreateAsync(CreateProjectAssignmentDto dto)
        {
            // Validate user exists
            var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new InvalidOperationException($"User with ID {dto.UserId} not found.");

            // Validate project exists and is active
            var project = await _unitOfWork.Projects.GetByIdAsync(dto.ProjectId);
            if (project == null)
                throw new InvalidOperationException($"Project with ID {dto.ProjectId} not found.");

            if (project.Status != Domain.Enums.ProjectStatus.Active)
                throw new InvalidOperationException($"Project '{project.Code}' is not active.");

            // Validate date range
            if (dto.EndDate.HasValue && dto.EndDate < dto.StartDate)
                throw new InvalidOperationException("End date cannot be before start date.");

            var assignment = _mapper.Map<ProjectAssignment>(dto);

            await _unitOfWork.ProjectAssignments.AddAsync(assignment);
            await _unitOfWork.SaveChangesAsync();

            // Reload with navigation properties
            var created = await _unitOfWork.ProjectAssignments.GetByIdAsync(assignment.Id);
            return _mapper.Map<ProjectAssignmentDto>(created);
        }

        public async Task<ProjectAssignmentDto?> UpdateAsync(int id, UpdateProjectAssignmentDto dto)
        {
            var assignment = await _unitOfWork.ProjectAssignments.GetByIdAsync(id);
            if (assignment == null) return null;

            // Validate date range
            if (dto.EndDate.HasValue && dto.EndDate < dto.StartDate)
                throw new InvalidOperationException("End date cannot be before start date.");

            assignment.StartDate = dto.StartDate;
            assignment.EndDate = dto.EndDate;

            _unitOfWork.ProjectAssignments.Update(assignment);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProjectAssignmentDto>(assignment);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var assignment = await _unitOfWork.ProjectAssignments.GetByIdAsync(id);
            if (assignment == null) return false;

            _unitOfWork.ProjectAssignments.Remove(assignment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsUserAssignedToProjectAsync(int userId, int projectId, DateTime date)
        {
            return await _unitOfWork.ProjectAssignments.IsUserAssignedToProjectAsync(userId, projectId, date);
        }
    }
}
