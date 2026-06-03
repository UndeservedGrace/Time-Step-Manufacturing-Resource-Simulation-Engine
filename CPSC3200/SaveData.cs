namespace CPSC3200;


public class SimulationSaveData
{
    public double PowerGrid { get; set; }
    public List<Dictionary<string, Item>> Containers { get; set; } = new();
    public List<CrafterSaveData> Crafters { get; set; } = new();
}

public class CrafterSaveData
{
    public int SourceIndex { get; set; }
    public int DestinationIndex { get; set; }

    public List<Recipe> Recipes { get; set; } = new();

    public string StrategyType { get; set; } = "";
    public int StrategyIndex { get; set; }

    public string PowerType { get; set; } = "";

    public int BatteryCharges { get; set; }
    public double PowerConsumption { get; set; }

    public int FuelContainerIndex { get; set; }
    public List<string> ValidFuels { get; set; } = new();
}