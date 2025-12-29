using Microsoft.Extensions.Logging;
using Timesheet.Application.DTOs.Timesheet;
using Timesheet.Application.Interfaces.Services;

namespace Timesheet.Application.Services
{
    /// <summary>
    /// Base decorator for TimesheetService that allows adding cross-cutting concerns.
    /// </summary>
    public abstract class TimesheetServiceWrapper : ITimesheetService
    {
        protected readonly ITimesheetService _innerService;

        protected TimesheetServiceWrapper(ITimesheetService innerService)
        {
            _innerService = innerService ?? throw new ArgumentNullException(nameof(innerService));
        }

        public virtual Task<TimesheetDto?> GetByIdAsync(int id) => _innerService.GetByIdAsync(id);
        public virtual Task<IEnumerable<TimesheetDto>> GetUserTimesheetsAsync(int userId) => _innerService.GetUserTimesheetsAsync(userId);
        public virtual Task<IEnumerable<TimesheetDto>> GetPendingTimesheetsAsync() => _innerService.GetPendingTimesheetsAsync();
        public virtual Task<TimesheetDto> CreateAsync(int userId, CreateTimesheetDto dto) => _innerService.CreateAsync(userId, dto);
        public virtual Task<TimesheetDto?> AddEntryAsync(int timesheetId, CreateTimesheetEntryDto dto) => _innerService.AddEntryAsync(timesheetId, dto);
        public virtual Task<bool> UpdateEntryAsync(int entryId, UpdateTimesheetEntryDto dto) => _innerService.UpdateEntryAsync(entryId, dto);
        public virtual Task<bool> DeleteEntryAsync(int entryId) => _innerService.DeleteEntryAsync(entryId);
        public virtual Task<bool> SubmitAsync(int timesheetId) => _innerService.SubmitAsync(timesheetId);
        public virtual Task<bool> ApproveAsync(int timesheetId) => _innerService.ApproveAsync(timesheetId);
        public virtual Task<bool> RejectAsync(int timesheetId, string comments) => _innerService.RejectAsync(timesheetId, comments);
    }

    /// <summary>
    /// Adds logging to all timesheet operations.
    /// </summary>
    public class LoggingTimesheetService : TimesheetServiceWrapper
    {
        private readonly ILogger<LoggingTimesheetService> _logger;

        public LoggingTimesheetService(ITimesheetService innerService, ILogger<LoggingTimesheetService> logger) 
            : base(innerService)
        {
            _logger = logger;
        }

        public override async Task<TimesheetDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Getting timesheet with ID: {TimesheetId}", id);
            var result = await base.GetByIdAsync(id);
            _logger.LogInformation("Retrieved timesheet {TimesheetId}: {Found}", id, result != null ? "Found" : "Not Found");
            return result;
        }

        public override async Task<TimesheetDto> CreateAsync(int userId, CreateTimesheetDto dto)
        {
            _logger.LogInformation("Creating timesheet for user {UserId} with {EntryCount} entries", userId, dto.Entries.Count);
            var result = await base.CreateAsync(userId, dto);
            _logger.LogInformation("Created timesheet {TimesheetId} for user {UserId}", result.Id, userId);
            return result;
        }

        public override async Task<bool> SubmitAsync(int timesheetId)
        {
            _logger.LogInformation("Submitting timesheet {TimesheetId}", timesheetId);
            var result = await base.SubmitAsync(timesheetId);
            _logger.LogInformation("Timesheet {TimesheetId} submit result: {Result}", timesheetId, result ? "Success" : "Failed");
            return result;
        }

        public override async Task<bool> ApproveAsync(int timesheetId)
        {
            _logger.LogInformation("Approving timesheet {TimesheetId}", timesheetId);
            var result = await base.ApproveAsync(timesheetId);
            _logger.LogInformation("Timesheet {TimesheetId} approval result: {Result}", timesheetId, result ? "Success" : "Failed");
            return result;
        }

        public override async Task<bool> RejectAsync(int timesheetId, string comments)
        {
            _logger.LogInformation("Rejecting timesheet {TimesheetId} with comments: {Comments}", timesheetId, comments);
            var result = await base.RejectAsync(timesheetId, comments);
            _logger.LogInformation("Timesheet {TimesheetId} rejection result: {Result}", timesheetId, result ? "Success" : "Failed");
            return result;
        }
    }

    /// <summary>
    /// Adds additional validation layer to timesheet operations.
    /// </summary>
    public class ValidatingTimesheetService : TimesheetServiceWrapper
    {
        private const double MAX_HOURS_PER_DAY = 24;
        private const double MAX_HOURS_PER_ENTRY = 12;

        public ValidatingTimesheetService(ITimesheetService innerService) : base(innerService)
        {
        }

        public override async Task<TimesheetDto> CreateAsync(int userId, CreateTimesheetDto dto)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            if (dto.SubmissionDate == default)
                throw new ArgumentException("Submission date is required.");

            ValidateEntries(dto.Entries);
            return await base.CreateAsync(userId, dto);
        }

        public override async Task<TimesheetDto?> AddEntryAsync(int timesheetId, CreateTimesheetEntryDto dto)
        {
            ValidateEntry(dto);
            return await base.AddEntryAsync(timesheetId, dto);
        }

        public override async Task<bool> UpdateEntryAsync(int entryId, UpdateTimesheetEntryDto dto)
        {
            if (dto.Hours <= 0)
                throw new ArgumentException("Hours must be greater than zero.");

            if (dto.Hours > MAX_HOURS_PER_ENTRY)
                throw new ArgumentException($"A single entry cannot exceed {MAX_HOURS_PER_ENTRY} hours.");

            return await base.UpdateEntryAsync(entryId, dto);
        }

        public override async Task<bool> RejectAsync(int timesheetId, string comments)
        {
            if (string.IsNullOrWhiteSpace(comments))
                throw new ArgumentException("Rejection comments are mandatory.");

            if (comments.Length < 10)
                throw new ArgumentException("Rejection comments must be at least 10 characters long.");

            return await base.RejectAsync(timesheetId, comments);
        }

        private void ValidateEntries(List<CreateTimesheetEntryDto> entries)
        {
            if (entries == null || !entries.Any())
                return;

            foreach (var entry in entries)
                ValidateEntry(entry);

            var duplicates = entries
                .GroupBy(e => new { e.ProjectId, Date = e.Date.Date })
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicates.Any())
            {
                var dup = duplicates.First().Key;
                throw new ArgumentException($"Duplicate entry found for Project {dup.ProjectId} on {dup.Date:d}.");
            }

            var dailyHours = entries
                .GroupBy(e => e.Date.Date)
                .Select(g => new { Date = g.Key, TotalHours = g.Sum(e => e.Hours) });

            foreach (var day in dailyHours)
            {
                if (day.TotalHours > MAX_HOURS_PER_DAY)
                    throw new ArgumentException($"Total hours for {day.Date:d} ({day.TotalHours}) exceeds maximum of {MAX_HOURS_PER_DAY}.");
            }
        }

        private void ValidateEntry(CreateTimesheetEntryDto dto)
        {
            if (dto.ProjectId <= 0)
                throw new ArgumentException("Invalid project ID.");

            if (dto.Hours <= 0)
                throw new ArgumentException("Hours must be greater than zero.");

            if (dto.Hours > MAX_HOURS_PER_ENTRY)
                throw new ArgumentException($"A single entry cannot exceed {MAX_HOURS_PER_ENTRY} hours.");

            if (dto.Date.Date > DateTime.UtcNow.Date)
                throw new ArgumentException("Cannot log hours for future dates.");
        }
    }
}
