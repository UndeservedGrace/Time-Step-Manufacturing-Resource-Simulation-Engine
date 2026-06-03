namespace CPSC3200;

public class BatteryPowerSource : IPowerSource
{
    private int _remainingCharges;

    public BatteryPowerSource(int charges)
    {
        _remainingCharges = charges;
    }
    public bool CanPower(Simulation simulation)
    {
        return _remainingCharges > 0;
    }
    public bool ConsumePower(Simulation simulation)
    {
        if (!CanPower(simulation))
        {
            return false;
        }

        _remainingCharges--;
        return true;
    }
    public IPowerSource Clone(
        IReadOnlyDictionary<Container, Container> containerMap)
    {
        return new BatteryPowerSource(
            _remainingCharges);
    }
    public string GetStatus()
    {
        return $"Battery Charges Remaining: {_remainingCharges}";
    }
    public int GetRemainingCharges()
    {
        return _remainingCharges;
    } 
}
