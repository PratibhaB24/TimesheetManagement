namespace Timesheet.Application.Calculations
{
    public enum CalculationType
    {
        Standard,
        Overtime,
        BillableOnly,
        WeeklyCapped
    }

    /// <summary>
    /// Selector for different hours calculation methods.
    /// </summary>
    public interface IHoursCalculationSelector
    {
        IHoursCalculationStrategy GetCalculation(CalculationType type);
        IEnumerable<IHoursCalculationStrategy> GetAllCalculations();
    }

    public class HoursCalculationSelector : IHoursCalculationSelector
    {
        private readonly Dictionary<CalculationType, IHoursCalculationStrategy> _calculations;

        public HoursCalculationSelector()
        {
            _calculations = new Dictionary<CalculationType, IHoursCalculationStrategy>
            {
                { CalculationType.Standard, new StandardHoursCalculation() },
                { CalculationType.Overtime, new OvertimeHoursCalculation() },
                { CalculationType.BillableOnly, new BillableOnlyCalculation() },
                { CalculationType.WeeklyCapped, new WeeklyCappedCalculation() }
            };
        }

        public IHoursCalculationStrategy GetCalculation(CalculationType type)
        {
            if (_calculations.TryGetValue(type, out var calculation))
                return calculation;

            throw new ArgumentException($"Unknown calculation type: {type}");
        }

        public IEnumerable<IHoursCalculationStrategy> GetAllCalculations() => _calculations.Values;
    }
}
