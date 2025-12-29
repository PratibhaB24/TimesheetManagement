using AutoMapper;
using Timesheet.Application.DTOs.Timesheet;
using Timesheet.Application.Interfaces.Repositories;
using Timesheet.Application.Interfaces.Services;
using Timesheet.Domain.Entities;
using Timesheet.Domain.Enums;

namespace Timesheet.Application.Services
{
    /// <summary>
    /// Timesheet Service Implementation.
    /// 
    /// BUSINESS RULES IMPLEMENTED:
    /// 1. Maximum 24 hours per day
    /// 2. No duplicate entries for same project code and date
    /// 3. Can only submit timesheets for assigned and active projects
    /// 4. Timesheet lifecycle: Draft → Submitted → Approved/Rejected
    /// </summary>
    public class TimesheetService : ITimesheetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProjectAssignmentService _assignmentService;

        private const double MAX_HOURS_PER_DAY = 24;

        public TimesheetService(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            IProjectAssignmentService assignmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _assignmentService = assignmentService;
        }

        public async Task<TimesheetDto?> GetByIdAsync(int id)
        {
            var timesheet = await _unitOfWork.Timesheets.GetTimesheetWithEntriesAsync(id);
            return timesheet != null ? _mapper.Map<TimesheetDto>(timesheet) : null;
        }

        public async Task<IEnumerable<TimesheetDto>> GetUserTimesheetsAsync(int userId)
        {
            var timesheets = await _unitOfWork.Timesheets.GetUserTimesheetsAsync(userId);
            return _mapper.Map<IEnumerable<TimesheetDto>>(timesheets);
        }

        public async Task<IEnumerable<TimesheetDto>> GetPendingTimesheetsAsync()
        {
            var timesheets = await _unitOfWork.Timesheets.GetPendingTimesheetsAsync();
            return _mapper.Map<IEnumerable<TimesheetDto>>(timesheets);
        }

        public async Task<TimesheetDto> CreateAsync(int userId, CreateTimesheetDto dto)
        {
            // Create timesheet
            var timesheet = new Domain.Entities.Timesheet
            {
                UserId = userId,
                SubmissionDate = dto.SubmissionDate,
                Status = TimesheetStatus.Draft
            };

            await _unitOfWork.Timesheets.AddAsync(timesheet);
            await _unitOfWork.SaveChangesAsync();

            // Add entries
            foreach (var entryDto in dto.Entries)
            {
                await AddEntryInternalAsync(timesheet.Id, userId, entryDto);
            }

            // Reload with entries
            var created = await _unitOfWork.Timesheets.GetTimesheetWithEntriesAsync(timesheet.Id);
            return _mapper.Map<TimesheetDto>(created);
        }

        public async Task<TimesheetDto?> AddEntryAsync(int timesheetId, CreateTimesheetEntryDto dto)
        {
            var timesheet = await _unitOfWork.Timesheets.GetByIdAsync(timesheetId);
            if (timesheet == null) return null;

            // Can only add entries to Draft timesheets
            if (timesheet.Status != TimesheetStatus.Draft)
                throw new InvalidOperationException("Cannot add entries to a submitted timesheet.");

            await AddEntryInternalAsync(timesheetId, timesheet.UserId, dto);

            var updated = await _unitOfWork.Timesheets.GetTimesheetWithEntriesAsync(timesheetId);
            return _mapper.Map<TimesheetDto>(updated);
        }

        private async Task AddEntryInternalAsync(int timesheetId, int userId, CreateTimesheetEntryDto dto)
        {
            // BUSINESS RULE 1: Check if user is assigned to the project on that date
            var isAssigned = await _assignmentService.IsUserAssignedToProjectAsync(userId, dto.ProjectId, dto.Date);
            if (!isAssigned)
                throw new InvalidOperationException($"User is not assigned to project {dto.ProjectId} on {dto.Date:d}.");

            // BUSINESS RULE 2: No duplicate entries for same project and date
            var entryExists = await _unitOfWork.TimesheetEntries.EntryExistsAsync(timesheetId, dto.ProjectId, dto.Date);
            if (entryExists)
                throw new InvalidOperationException($"Entry already exists for project {dto.ProjectId} on {dto.Date:d}.");

            // BUSINESS RULE 3: Maximum 24 hours per day
            var totalHoursForDay = await _unitOfWork.TimesheetEntries.GetTotalHoursForDateAsync(timesheetId, dto.Date);
            if (totalHoursForDay + dto.Hours > MAX_HOURS_PER_DAY)
                throw new InvalidOperationException($"Total hours for {dto.Date:d} cannot exceed {MAX_HOURS_PER_DAY} hours.");

            // Validate hours
            if (dto.Hours <= 0 || dto.Hours > MAX_HOURS_PER_DAY)
                throw new InvalidOperationException($"Hours must be between 0 and {MAX_HOURS_PER_DAY}.");

            var entry = _mapper.Map<TimesheetEntry>(dto);
            entry.TimesheetId = timesheetId;

            await _unitOfWork.TimesheetEntries.AddAsync(entry);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateEntryAsync(int entryId, UpdateTimesheetEntryDto dto)
        {
            var entry = await _unitOfWork.TimesheetEntries.GetByIdAsync(entryId);
            if (entry == null) return false;

            var timesheet = await _unitOfWork.Timesheets.GetByIdAsync(entry.TimesheetId);
            if (timesheet == null || timesheet.Status != TimesheetStatus.Draft)
                throw new InvalidOperationException("Cannot update entries of a submitted timesheet.");

            // Check max hours constraint
            var totalHoursForDay = await _unitOfWork.TimesheetEntries.GetTotalHoursForDateAsync(entry.TimesheetId, entry.Date);
            var hoursWithoutThisEntry = totalHoursForDay - entry.Hours;
            
            if (hoursWithoutThisEntry + dto.Hours > MAX_HOURS_PER_DAY)
                throw new InvalidOperationException($"Total hours for {entry.Date:d} cannot exceed {MAX_HOURS_PER_DAY} hours.");

            entry.Hours = dto.Hours;
            entry.Description = dto.Description;

            _unitOfWork.TimesheetEntries.Update(entry);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteEntryAsync(int entryId)
        {
            var entry = await _unitOfWork.TimesheetEntries.GetByIdAsync(entryId);
            if (entry == null) return false;

            var timesheet = await _unitOfWork.Timesheets.GetByIdAsync(entry.TimesheetId);
            if (timesheet == null || timesheet.Status != TimesheetStatus.Draft)
                throw new InvalidOperationException("Cannot delete entries from a submitted timesheet.");

            _unitOfWork.TimesheetEntries.Remove(entry);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SubmitAsync(int timesheetId)
        {
            var timesheet = await _unitOfWork.Timesheets.GetTimesheetWithEntriesAsync(timesheetId);
            if (timesheet == null) return false;

            if (timesheet.Status != TimesheetStatus.Draft)
                throw new InvalidOperationException("Only draft timesheets can be submitted.");

            if (!timesheet.Entries.Any())
                throw new InvalidOperationException("Cannot submit an empty timesheet.");

            timesheet.Status = TimesheetStatus.Submitted;
            timesheet.SubmissionDate = DateTime.UtcNow;

            _unitOfWork.Timesheets.Update(timesheet);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ApproveAsync(int timesheetId)
        {
            var timesheet = await _unitOfWork.Timesheets.GetByIdAsync(timesheetId);
            if (timesheet == null) return false;

            if (timesheet.Status != TimesheetStatus.Submitted)
                throw new InvalidOperationException("Only submitted timesheets can be approved.");

            timesheet.Status = TimesheetStatus.Approved;
            timesheet.RejectionComments = null;

            _unitOfWork.Timesheets.Update(timesheet);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectAsync(int timesheetId, string comments)
        {
            if (string.IsNullOrWhiteSpace(comments))
                throw new InvalidOperationException("Rejection comments are required.");

            var timesheet = await _unitOfWork.Timesheets.GetByIdAsync(timesheetId);
            if (timesheet == null) return false;

            if (timesheet.Status != TimesheetStatus.Submitted)
                throw new InvalidOperationException("Only submitted timesheets can be rejected.");

            timesheet.Status = TimesheetStatus.Rejected;
            timesheet.RejectionComments = comments;

            _unitOfWork.Timesheets.Update(timesheet);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
