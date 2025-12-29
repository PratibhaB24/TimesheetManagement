using Microsoft.AspNetCore.Mvc;
using Timesheet.Application.DTOs.Common;
using Timesheet.Application.DTOs.Timesheet;
using Timesheet.Application.Interfaces.Services;

namespace Timesheet.Api.Controllers
{
    /// <summary>
    /// Timesheet Management Controller.
    /// Employee role: Submit weekly timesheets with entries.
    /// Manager role: Approve or reject timesheets.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TimesheetsController : ControllerBase
    {
        private readonly ITimesheetService _timesheetService;

        public TimesheetsController(ITimesheetService timesheetService)
        {
            _timesheetService = timesheetService;
        }

        /// <summary>
        /// Get timesheet by ID with all entries.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TimesheetDto>>> GetById(int id)
        {
            var timesheet = await _timesheetService.GetByIdAsync(id);
            if (timesheet == null)
                return NotFound(ApiResponse<TimesheetDto>.ErrorResponse("Timesheet not found."));

            return Ok(ApiResponse<TimesheetDto>.SuccessResponse(timesheet));
        }

        /// <summary>
        /// Get all timesheets for a specific user.
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TimesheetDto>>>> GetUserTimesheets(int userId)
        {
            var timesheets = await _timesheetService.GetUserTimesheetsAsync(userId);
            return Ok(ApiResponse<IEnumerable<TimesheetDto>>.SuccessResponse(timesheets));
        }

        /// <summary>
        /// Get all pending (submitted) timesheets for manager review.
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TimesheetDto>>>> GetPendingTimesheets()
        {
            var timesheets = await _timesheetService.GetPendingTimesheetsAsync();
            return Ok(ApiResponse<IEnumerable<TimesheetDto>>.SuccessResponse(timesheets));
        }

        /// <summary>
        /// Create a new timesheet (Employee).
        /// In production, userId would come from authenticated user context.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TimesheetDto>>> Create(
            [FromQuery] int userId, 
            [FromBody] CreateTimesheetDto dto)
        {
            try
            {
                var timesheet = await _timesheetService.CreateAsync(userId, dto);
                return CreatedAtAction(nameof(GetById), new { id = timesheet.Id },
                    ApiResponse<TimesheetDto>.SuccessResponse(timesheet, "Timesheet created successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<TimesheetDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Add an entry to an existing draft timesheet.
        /// </summary>
        [HttpPost("{timesheetId}/entries")]
        public async Task<ActionResult<ApiResponse<TimesheetDto>>> AddEntry(
            int timesheetId, 
            [FromBody] CreateTimesheetEntryDto dto)
        {
            try
            {
                var timesheet = await _timesheetService.AddEntryAsync(timesheetId, dto);
                if (timesheet == null)
                    return NotFound(ApiResponse<TimesheetDto>.ErrorResponse("Timesheet not found."));

                return Ok(ApiResponse<TimesheetDto>.SuccessResponse(timesheet, "Entry added successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<TimesheetDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Update an existing timesheet entry.
        /// </summary>
        [HttpPut("entries/{entryId}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateEntry(
            int entryId, 
            [FromBody] UpdateTimesheetEntryDto dto)
        {
            try
            {
                var result = await _timesheetService.UpdateEntryAsync(entryId, dto);
                if (!result)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Entry not found."));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Entry updated successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Delete a timesheet entry.
        /// </summary>
        [HttpDelete("entries/{entryId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteEntry(int entryId)
        {
            try
            {
                var result = await _timesheetService.DeleteEntryAsync(entryId);
                if (!result)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Entry not found."));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Entry deleted successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Submit a draft timesheet for approval.
        /// </summary>
        [HttpPost("{id}/submit")]
        public async Task<ActionResult<ApiResponse<bool>>> Submit(int id)
        {
            try
            {
                var result = await _timesheetService.SubmitAsync(id);
                if (!result)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Timesheet not found."));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Timesheet submitted successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Approve a submitted timesheet (Manager only).
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<ActionResult<ApiResponse<bool>>> Approve(int id)
        {
            try
            {
                var result = await _timesheetService.ApproveAsync(id);
                if (!result)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Timesheet not found."));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Timesheet approved successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Reject a submitted timesheet (Manager only).
        /// Rejection requires mandatory comments.
        /// </summary>
        [HttpPost("{id}/reject")]
        public async Task<ActionResult<ApiResponse<bool>>> Reject(int id, [FromBody] ApproveRejectTimesheetDto dto)
        {
            try
            {
                var result = await _timesheetService.RejectAsync(id, dto.RejectionComments ?? "");
                if (!result)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Timesheet not found."));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Timesheet rejected."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }
    }
}
