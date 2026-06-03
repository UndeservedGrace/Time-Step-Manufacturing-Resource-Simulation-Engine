using System.Numerics;

namespace CPSC3200;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Text.Json;

public class Simulation
{
    
    List<Crafter> _crafters = new List<Crafter>();
    List<Container> _containers = new List<Container>();

    public Simulation(){}
    private double _powerGrid;

    public double PowerGrid
    {
        get => _powerGrid;
        set => _powerGrid = value;
    }
    
    /// Creates a container and stores it in the simulation.
    public Container CreateContainer(Dictionary<string, Item> items)
    {
        Container newContainer = new Container(items);
        _containers.Add(newContainer);
        return newContainer;
    }
    
    // Prints the contents of the crafters
    public string GetCraftersSummary()
    {
        StringBuilder sb = new();

        for (int i = 0; i < _crafters.Count; i++)
        {
            sb.AppendLine($"Crafter #{i}");
            sb.AppendLine(_crafters[i].GetSummary());
            sb.AppendLine();
        }

        return sb.ToString();
    }
    
    //Gets a given crafters index (needed for SignleActiveStrategy)
    public Crafter? GetCrafter(int index)
    {
        if (index < 0 || index >= _crafters.Count)
        {
            return null;
        }

        return _crafters[index];
    }
    /// Creates a crafter and stores it in the simulation.
   
    public Crafter CreateCrafter(
        List<Recipe> recipes,
        Container destination,
        Container source,
        IRecipeStrategy strategy,
        IPowerSource powerSource)
    {
        Crafter newCrafter =
            new Crafter(
                recipes,
                source,
                destination,
                strategy,
                powerSource);

        _crafters.Add(newCrafter);

        return newCrafter;
    }

    /// Advances all crafters by one simulation tick.
    public void Step()
    {
        foreach (Crafter crafter in _crafters)
        {
            crafter.Advance(this);
        }
    }
    
    public bool TryConsumePower(double amount)
    {
        // Power producers always succeed
        if (amount < 0)
        {
            _powerGrid -= amount; // subtracting negative adds power
            return true;
        }

        // Not enough power available
        if (_powerGrid < amount)
        {
            return false;
        }

        _powerGrid -= amount;
        return true;
    }
    
    /// Prints all containers and crafters.
    public string GetSummary()
    {
        StringBuilder sb = new();

        sb.AppendLine("=== Simulation ===");
        sb.AppendLine($"Grid Power: {PowerGrid}");

        for (int i = 0; i < _containers.Count; i++)
        {
            sb.AppendLine($"Container #{i}");
            sb.AppendLine(_containers[i].GetSummary());
        }

        for (int i = 0; i < _crafters.Count; i++)
        {
            sb.AppendLine($"Crafter #{i}");
            sb.AppendLine(_crafters[i].GetSummary());
        }

        return sb.ToString();
    }
    
    public Simulation(Simulation other)
    {
        _powerGrid = other._powerGrid;
        Dictionary<Container, Container> containerMap = new();
        // Clone containers
        foreach (Container oldContainer in other._containers)
        {
            Container newContainer = oldContainer.Clone();

            _containers.Add(newContainer);

            containerMap[oldContainer] = newContainer;
        }

        // Clone crafters
        foreach (Crafter oldCrafter in other._crafters)
        {
            List<Recipe> recipes = new();

            foreach (Recipe recipe in oldCrafter.GetRecipes())
            {
                recipes.Add(recipe.Clone());
            }

            Container newSource =
                containerMap[oldCrafter.GetSource()!];

            Container newDestination =
                containerMap[oldCrafter.GetDestination()!];

            IRecipeStrategy strategy =
                oldCrafter
                    .GetStrategy()
                    .Clone();

            IPowerSource powerSource =
                oldCrafter
                    .GetPowerSource()
                    .Clone(containerMap);

            Crafter newCrafter =
                new Crafter(
                    recipes,
                    newSource,
                    newDestination,
                    strategy,
                    powerSource);

            _crafters.Add(newCrafter);
        }
    }
    public void Save(string filename)
{
    SimulationSaveData data = new SimulationSaveData();
    data.PowerGrid = PowerGrid;

    foreach (Container container in _containers)
    {
        data.Containers.Add(container.GetItems());
    }

    foreach (Crafter crafter in _crafters)
    {
        CrafterSaveData savedCrafter = new CrafterSaveData();

        int sourceIndex = _containers.IndexOf(crafter.GetSource()!);
        int destinationIndex = _containers.IndexOf(crafter.GetDestination()!);

        if (sourceIndex == -1)
        {
            throw new InvalidOperationException(
                "Crafter source container is not registered in this simulation.");
        }

        if (destinationIndex == -1)
        {
            throw new InvalidOperationException(
                "Crafter destination container is not registered in this simulation.");
        }

        savedCrafter.SourceIndex = sourceIndex;
        savedCrafter.DestinationIndex = destinationIndex;

        foreach (Recipe recipe in crafter.GetRecipes())
        {
            savedCrafter.Recipes.Add(recipe.Clone());
        }

        IRecipeStrategy strategy = crafter.GetStrategy();

        if (strategy is SingleActiveStrategy single)
        {
            savedCrafter.StrategyType = "SingleActive";
            savedCrafter.StrategyIndex = single.GetActiveRecipeIndex();
        }
        else if (strategy is RotationStrategy rotation)
        {
            savedCrafter.StrategyType = "Rotation";
            savedCrafter.StrategyIndex = rotation.GetCurrentIndex();
        }
        else if (strategy is PriorityStrategy)
        {
            savedCrafter.StrategyType = "Priority";
        }
        else
        {
            throw new InvalidOperationException(
                $"Unsupported strategy type: {strategy.GetType().Name}");
        }

        IPowerSource power = crafter.GetPowerSource();

        if (power is BatteryPowerSource battery)
        {
            savedCrafter.PowerType = "Battery";
            savedCrafter.BatteryCharges = battery.GetRemainingCharges();
        }
        else if (power is ElectricPowerSource electric)
        {
            savedCrafter.PowerType = "Electric";
            savedCrafter.PowerConsumption = electric.GetPowerConsumption();
        }
        else if (power is FuelPowerSource fuel)
        {
            int fuelIndex = _containers.IndexOf(fuel.GetFuelContainer());

            if (fuelIndex == -1)
            {
                throw new InvalidOperationException(
                    "Fuel container is not registered in this simulation. " +
                    "Create fuel containers using simulation.CreateContainer(...).");
            }

            savedCrafter.PowerType = "Fuel";
            savedCrafter.FuelContainerIndex = fuelIndex;
            savedCrafter.ValidFuels = fuel.GetValidFuelTypes();
        }
        else
        {
            throw new InvalidOperationException(
                $"Unsupported power source type: {power.GetType().Name}");
        }

        data.Crafters.Add(savedCrafter);
    }

    JsonSerializerOptions options = new JsonSerializerOptions
    {
        WriteIndented = true
    };
    
    string json = JsonSerializer.Serialize(data, options);
    File.WriteAllText(filename, json);
}

public static Simulation Load(string filename)
{
    string json = File.ReadAllText(filename);

    SimulationSaveData? data =
        JsonSerializer.Deserialize<SimulationSaveData>(json);

    if (data == null)
    {
        throw new InvalidOperationException("Could not load simulation.");
    }

    Simulation simulation = new Simulation();
    simulation.PowerGrid = data.PowerGrid;

    List<Container> loadedContainers = new();

    foreach (Dictionary<string, Item> items in data.Containers)
    {
        loadedContainers.Add(simulation.CreateContainer(items));
    }

    foreach (CrafterSaveData savedCrafter in data.Crafters)
    {
        if (savedCrafter.SourceIndex < 0 ||
            savedCrafter.SourceIndex >= loadedContainers.Count)
        {
            throw new InvalidOperationException(
                $"Invalid source container index: {savedCrafter.SourceIndex}");
        }

        if (savedCrafter.DestinationIndex < 0 ||
            savedCrafter.DestinationIndex >= loadedContainers.Count)
        {
            throw new InvalidOperationException(
                $"Invalid destination container index: {savedCrafter.DestinationIndex}");
        }

        IRecipeStrategy strategy;

        if (savedCrafter.StrategyType == "SingleActive")
        {
            strategy = new SingleActiveStrategy(savedCrafter.StrategyIndex);
        }
        else if (savedCrafter.StrategyType == "Rotation")
        {
            strategy = new RotationStrategy(savedCrafter.StrategyIndex);
        }
        else if (savedCrafter.StrategyType == "Priority")
        {
            strategy = new PriorityStrategy();
        }
        else
        {
            throw new InvalidOperationException(
                $"Unknown strategy type: {savedCrafter.StrategyType}");
        }

        IPowerSource powerSource;

        if (savedCrafter.PowerType == "Battery")
        {
            powerSource =
                new BatteryPowerSource(savedCrafter.BatteryCharges);
        } else if (savedCrafter.PowerType == "Electric")
        {
            powerSource =
                new ElectricPowerSource(savedCrafter.PowerConsumption);
        }
        else if (savedCrafter.PowerType == "Fuel")
        {
            if (savedCrafter.FuelContainerIndex < 0 ||
                savedCrafter.FuelContainerIndex >= loadedContainers.Count)
            {
                throw new InvalidOperationException(
                    $"Invalid fuel container index: {savedCrafter.FuelContainerIndex}");
            }

            powerSource =
                new FuelPowerSource(
                    loadedContainers[savedCrafter.FuelContainerIndex],
                    savedCrafter.ValidFuels);
        }
        else
        {
            throw new InvalidOperationException(
                $"Unknown power source type: {savedCrafter.PowerType}");
        }

        List<Recipe> recipes = new();

        foreach (Recipe recipe in savedCrafter.Recipes)
        {
            recipes.Add(recipe.Clone());
        }

        simulation.CreateCrafter(
            recipes,
            loadedContainers[savedCrafter.DestinationIndex],
            loadedContainers[savedCrafter.SourceIndex],
            strategy,
            powerSource);
    }
    return simulation;
    }
}