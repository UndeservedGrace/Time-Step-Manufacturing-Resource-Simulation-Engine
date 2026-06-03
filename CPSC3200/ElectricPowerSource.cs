namespace CPSC3200;

public class ElectricPowerSource : IPowerSource
{
    private readonly double _powerConsumption;
    public ElectricPowerSource(double powerConsumption)
    {
        _powerConsumption = powerConsumption;
    }
    public bool CanPower(Simulation simulation)
    {
        return _powerConsumption < 0 || simulation.PowerGrid >= _powerConsumption;
    }
    public bool ConsumePower(Simulation simulation)
    {
        return simulation.TryConsumePower(_powerConsumption);
    }

    public string GetStatus()
    {
        return $"Electric ({_powerConsumption} power/tick)";
    }

    public IPowerSource Clone(
        IReadOnlyDictionary<Container, Container> containerMap)
    {
        return new ElectricPowerSource(_powerConsumption);
    }
    
    public double GetPowerConsumption()
    {
        return _powerConsumption;
    }
}