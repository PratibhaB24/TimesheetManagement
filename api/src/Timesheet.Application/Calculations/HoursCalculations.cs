using Timesheet.Domain.Entities;

namespace Timesheet.Application.Calculations
{
    /// <summary>
    /// Interface for different hours calculation methods.
    /// Allows switching calculation algorithms at runtime.
    /// </summary>
    public interface IHoursCalculationStrategy
    {
        string StrategyName { get; }
        double CalculateHours(IEnumerable<TimesheetEntry> entries);
        double CalculateBillableAmount(IEnumerable<TimesheetEntry> entries, double hourlyRate);
    }

    /// <summary>
    /// Standard hours calculation - simply sums all hours.
    /// </summary>
    public class StandardHoursCalculation : IHoursCalculationStrategy
    {
        public string StrategyName => "Standard";

        public double CalculateHours(IEnumerable<TimesheetEntry> entries)
        {
            return entries.Sum(e => e.Hours);
        }

        public double CalculateBillableAmount(IEnumerable<TimesheetEntry> entries, double hourlyRate)
        {
            return Math.Round(CalculateHours(entries) * hourlyRate, 2);
        }
    }

    /// <summary>
    /// Overtime calculation - hours over 8 per day are counted as 1.5x.
    /// </summary>
    public class OvertimeHoursCalculation : IHoursCalculationStrategy
    {
        private const double REGULAR_HOURS_PER_DAY = 8;
        private const double OVERTIME_MULTIPLIER = 1.5;

        public string StrategyName => "Overtime";

        public double CalculateHours(IEnumerable<TimesheetEntry> entries)
        {
            var dailyHours = entries
                .GroupBy(e => e.Date.Date)
                .Select(g => g.Sum(e => e.Hours));

            double totalEffectiveHours = 0;
            foreach (var hours in dailyHours)
            {
                if (hours <= REGULAR_HOURS_PER_DAY)
                    totalEffectiveHours += hours;
                else
                    totalEffectiveHours += REGULAR_HOURS_PER_DAY + (hours - REGULAR_HOURS_PER_DAY) * OVERTIME_MULTIPLIER;
            }

            return Math.Round(totalEffectiveHours, 2);
        }

        public double CalculateBillableAmount(IEnumerable<TimesheetEntry> entries, double hourlyRate)
        {
            return Math.Round(CalculateHours(entries) * hourlyRate, 2);
        }
    }

    /// <summary>
    /// Billable-only calculation - only counts hours from billable projects.
    /// </summary>
    public class BillableOnlyCalculation : IHoursCalculationStrategy
    {
        public string StrategyName => "Billable Only";

        public double CalculateHours(IEnumerable<TimesheetEntry> entries)
        {
            return entries
                .Where(e => e.Project != null && e.Project.IsBillable)
                .Sum(e => e.Hours);
        }

        public double CalculateBillableAmount(IEnumerable<TimesheetEntry> entries, double hourlyRate)
        {
            return Math.Round(CalculateHours(entries) * hourlyRate, 2);
        }
    }

    /// <summary>
    /// Weekly capped calculation - caps hours at 40 per week.
    /// </summary>
    public class WeeklyCappedCalculation : IHoursCalculationStrategy
    {
        private const double MAX_WEEKLY_HOURS = 40;

        public string StrategyName => "Weekly Capped (40 hrs)";

        public double CalculateHours(IEnumerable<TimesheetEntry> entries)
        {
            return Math.Min(entries.Sum(e => e.Hours), MAX_WEEKLY_HOURS);
        }

        public double CalculateBillableAmount(IEnumerable<TimesheetEntry> entries, double hourlyRate)
        {
            return Math.Round(CalculateHours(entries) * hourlyRate, 2);
        }
    }
}
