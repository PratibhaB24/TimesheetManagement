using Microsoft.AspNetCore.Mvc;
using Timesheet.Application.DTOs.Common;
using Timesheet.Application.DTOs.Project;
using Timesheet.Application.Interfaces.Services;

namespace Timesheet.Api.Controllers
{
    /// <summary>
    /// Project Management Controller.
    /// Manager role: Create, update, activate, and deactivate project codes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Get all projects.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectDto>>>> GetAll()
        {
            var projects = await _projectService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ProjectDto>>.SuccessResponse(projects));
        }

        /// <summary>
        /// Get active projects only.
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectDto>>>> GetActiveProjects()
        {
            var projects = await _projectService.GetActiveProjectsAsync();
            return Ok(ApiResponse<IEnumerable<ProjectDto>>.SuccessResponse(projects));
        }

        /// <summary>
        /// Get project by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProjectDto>>> GetById(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
                return NotFound(ApiResponse<ProjectDto>.ErrorResponse("Project not found."));

            return Ok(ApiResponse<ProjectDto>.SuccessResponse(project));
        }

        /// <summary>
        /// Get project by code.
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<ActionResult<ApiResponse<ProjectDto>>> GetByCode(string code)
        {
            var project = await _projectService.GetByCodeAsync(code);
            if (project == null)
                return NotFound(ApiResponse<ProjectDto>.ErrorResponse("Project not found."));

            return Ok(ApiResponse<ProjectDto>.SuccessResponse(project));
        }

        /// <summary>
        /// Create a new project (Manager only).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProjectDto>>> Create([FromBody] CreateProjectDto dto)
        {
            try
            {
                var project = await _projectService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = project.Id },
                    ApiResponse<ProjectDto>.SuccessResponse(project, "Project created successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<ProjectDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Update an existing project (Manager only).
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProjectDto>>> Update(int id, [FromBody] UpdateProjectDto dto)
        {
            var project = await _projectService.UpdateAsync(id, dto);
            if (project == null)
                return NotFound(ApiResponse<ProjectDto>.ErrorResponse("Project not found."));

            return Ok(ApiResponse<ProjectDto>.SuccessResponse(project, "Project updated successfully."));
        }

        /// <summary>
        /// Activate a project (Manager only).
        /// </summary>
        [HttpPut("{id}/activate")]
        public async Task<ActionResult<ApiResponse<bool>>> Activate(int id)
        {
            var result = await _projectService.ActivateAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Project not found."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Project activated successfully."));
        }

        /// <summary>
        /// Deactivate a project (Manager only).
        /// </summary>
        [HttpPut("{id}/deactivate")]
        public async Task<ActionResult<ApiResponse<bool>>> Deactivate(int id)
        {
            var result = await _projectService.DeactivateAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Project not found."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Project deactivated successfully."));
        }
    }
}
