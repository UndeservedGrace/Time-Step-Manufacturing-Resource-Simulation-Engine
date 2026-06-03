namespace CPSC3200;

public class FuelPowerSource : IPowerSource
{
    private readonly Container _fuelContainer;

    private readonly HashSet<string>
        _validFuelTypes;
    public FuelPowerSource(
        Container fuelContainer,
        IEnumerable<string> validFuelTypes)
    {
        _fuelContainer = fuelContainer;
        _validFuelTypes = new HashSet<string>(validFuelTypes);
    }
    public bool CanPower(Simulation simulation)
    {
        foreach (string fuel in _validFuelTypes)
        {
            if (_fuelContainer.HasItem(fuel, 1))
            {
                return true;
            }
        }
        return false;
    }
    
    public bool ConsumePower(Simulation simulation)
    {
        foreach (string fuel in _validFuelTypes)
        {
            if (_fuelContainer.RemoveItem(fuel, 1))
            {
                return true;
            }
        }

        return false;
    }

    public IPowerSource Clone(
        IReadOnlyDictionary<Container, Container> containerMap)
    {
        return new FuelPowerSource(
            containerMap[_fuelContainer],
            _validFuelTypes);
    }
    public string GetStatus()
    {
        return $"Fuel Powered ({string.Join(", ", _validFuelTypes)})";
    }
    public Container GetFuelContainer()
    {
        return _fuelContainer;
    }

    public List<string> GetValidFuelTypes()
    {
        return new List<string>(_validFuelTypes);
    }
}