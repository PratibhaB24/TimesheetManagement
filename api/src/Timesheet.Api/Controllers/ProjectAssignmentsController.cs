using Microsoft.AspNetCore.Mvc;
using Timesheet.Application.DTOs.Common;
using Timesheet.Application.DTOs.ProjectAssignment;
using Timesheet.Application.Interfaces.Services;

namespace Timesheet.Api.Controllers
{
    /// <summary>
    /// Project Assignment Controller.
    /// Manager role: Assign project codes to employees with start/end dates.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectAssignmentsController : ControllerBase
    {
        private readonly IProjectAssignmentService _assignmentService;

        public ProjectAssignmentsController(IProjectAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        /// <summary>
        /// Get all project assignments.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectAssignmentDto>>>> GetAll()
        {
            var assignments = await _assignmentService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ProjectAssignmentDto>>.SuccessResponse(assignments));
        }

        /// <summary>
        /// Get assignment by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProjectAssignmentDto>>> GetById(int id)
        {
            var assignment = await _assignmentService.GetByIdAsync(id);
            if (assignment == null)
                return NotFound(ApiResponse<ProjectAssignmentDto>.ErrorResponse("Assignment not found."));

            return Ok(ApiResponse<ProjectAssignmentDto>.SuccessResponse(assignment));
        }

        /// <summary>
        /// Get assignments for a specific user.
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectAssignmentDto>>>> GetUserAssignments(int userId)
        {
            var assignments = await _assignmentService.GetUserAssignmentsAsync(userId);
            return Ok(ApiResponse<IEnumerable<ProjectAssignmentDto>>.SuccessResponse(assignments));
        }

        /// <summary>
        /// Get assignments for a specific project.
        /// </summary>
        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectAssignmentDto>>>> GetProjectAssignments(int projectId)
        {
            var assignments = await _assignmentService.GetProjectAssignmentsAsync(projectId);
            return Ok(ApiResponse<IEnumerable<ProjectAssignmentDto>>.SuccessResponse(assignments));
        }

        /// <summary>
        /// Create a new project assignment (Manager only).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProjectAssignmentDto>>> Create([FromBody] CreateProjectAssignmentDto dto)
        {
            try
            {
                var assignment = await _assignmentService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = assignment.Id },
                    ApiResponse<ProjectAssignmentDto>.SuccessResponse(assignment, "Assignment created successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<ProjectAssignmentDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Update an existing assignment (Manager only).
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProjectAssignmentDto>>> Update(int id, [FromBody] UpdateProjectAssignmentDto dto)
        {
            try
            {
                var assignment = await _assignmentService.UpdateAsync(id, dto);
                if (assignment == null)
                    return NotFound(ApiResponse<ProjectAssignmentDto>.ErrorResponse("Assignment not found."));

                return Ok(ApiResponse<ProjectAssignmentDto>.SuccessResponse(assignment, "Assignment updated successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<ProjectAssignmentDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Delete an assignment (Manager only).
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await _assignmentService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Assignment not found."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Assignment deleted successfully."));
        }
    }
}
