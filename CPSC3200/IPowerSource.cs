namespace CPSC3200;

public interface IPowerSource
{
    bool ConsumePower(Simulation simulation);

    IPowerSource Clone(IReadOnlyDictionary<Container, Container> containerMap);
    string GetStatus();
    bool CanPower(Simulation simulation);

}