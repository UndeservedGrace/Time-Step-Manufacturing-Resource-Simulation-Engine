namespace CPSC3200;
using System.Text;

public class Crafter
{
    private List<Recipe> _recipes;
    private Container? _source;
    private Container? _destination;
    private IRecipeStrategy _strategy;
    private IPowerSource _powerSource;
    private Recipe? _currentRecipe;
    private int _progress = 0;
    public Recipe? CurrentRecipe => _currentRecipe;

    // -----------------------------
    // Constructor
    // -----------------------------
    public Crafter(
        List<Recipe> recipes,
        Container source,
        Container destination,
        IRecipeStrategy strategy,
        IPowerSource powerSource)
    {
        _recipes = recipes;
        _source = source;
        _destination = destination;
        _strategy = strategy;
        _powerSource = powerSource;
    }
    
    // Initialize the object 
    public void Initialize(
        List<Recipe> recipes,
        Container source,
        Container destination,
        IRecipeStrategy strategy,
        IPowerSource powerSource)
    {
        _recipes = recipes;
        _source = source;
        _destination = destination;
        _strategy = strategy;
        _powerSource = powerSource;
    }
    
    // -----------------------------
    // Getters
    // -----------------------------
    // Get destination (destination container)
    public Container? GetDestination()
    {
        return _destination;
    }

    // Get current power source
    public IPowerSource GetPowerSource()
    {
        return _powerSource;
    }
    
    // Get Current Recipe
    public Recipe? GetCurrentRecipe()
    {
        return _currentRecipe;
    }

    // Get Recipe
    public List<Recipe> GetRecipes()
    {
        return _recipes;
    }
    
    // Get Strategy
    public IRecipeStrategy GetStrategy()
    {
        return _strategy;
    }
    
    // Get input (source container)
    public Container? GetSource()
    {
        return _source;
    }

    // Clone crafter
    public Crafter Clone(
        IReadOnlyDictionary<Container, Container> containerMap)
    {
        List<Recipe> recipes = new();

        foreach (Recipe recipe in _recipes)
        {
            recipes.Add(recipe.Clone());
        }

        return new Crafter(
            recipes,
            containerMap[_source],
            containerMap[_destination],
            _strategy.Clone(),
            _powerSource.Clone(containerMap)
        );
    }
    
   
    // -----------------------------
    // Updating Crafter
    // -----------------------------
    public void Advance(Simulation simulation)
    {
        // Not Currently Crafting
        if (_currentRecipe == null)
        {
            if (!_powerSource.CanPower(simulation))
            {
                return;
            }

            Recipe? selected =
                _strategy.SelectRecipe(
                    _recipes,
                    _source);

            if (selected == null)
            {
                return;
            }

            if (!ConsumeInputs(selected))
            {
                return;
            }

            _powerSource.ConsumePower(simulation);

            _currentRecipe = selected;
            _progress = 0;
        }
        _progress++;
        if (_progress >= _currentRecipe.CraftTime)
        {
            foreach (Item output in _currentRecipe.Outputs)
            {
                _destination.AddItem(
                    output.Name,
                    output.Quantity);
            }

            _currentRecipe = null;
            _progress = 0;
        }
    }
    
    // If there is sufficient resources consumes all necessary items
    private bool ConsumeInputs(Recipe recipe)
    {
        if (_source == null) { return false; }

        foreach (Item input in recipe.Inputs)
        {
            if (!_source.HasItem(input.Name, input.Quantity))
            {
                return false;
            }
        }

        foreach (Item input in recipe.Inputs)
        {
            _source.RemoveItem(input.Name, input.Quantity);
        }
        return true;
    }
    
    // -----------------------------
    // Other
    // -----------------------------
    
    // Gets a summary of crafter
    public string GetSummary()
    {
        StringBuilder sb = new();

        for (int i = 0; i < _recipes.Count; i++)
        {
            Recipe recipe = _recipes[i];

            sb.AppendLine($"Recipe #{i}");

            sb.AppendLine("Inputs:");

            foreach (Item input in recipe.Inputs)
            {
                sb.AppendLine(
                    $"{input.Name} x{input.Quantity}");
            }

            sb.AppendLine("Outputs:");

            foreach (Item output in recipe.Outputs)
            {
                sb.AppendLine(
                    $"{output.Name} x{output.Quantity}");
            }
            sb.AppendLine();
        }

        sb.AppendLine(_powerSource.GetStatus());

        if (_currentRecipe == null)
        {
            sb.AppendLine("Idle");
        }
        else
        {
            sb.AppendLine(
                $"Crafting ({_progress}/{_currentRecipe.CraftTime})");
        }

        return sb.ToString();
    }
}


    
