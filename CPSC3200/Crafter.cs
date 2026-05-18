namespace CPSC3200;

public class Crafter
{
    private List<Recipe> _recipes;
    private Container? _source;
    private Container? _destination;
    private IRecipeStrategy _strategy;
    private Recipe? _currentRecipe;
    private int _progress = 0;
    public Recipe? CurrentRecipe => _currentRecipe;

    // -----------------------------
    // Constructor
    // -----------------------------
    public Crafter(
        List<Recipe> recipes,
        Container? source,
        Container? destination,
        IRecipeStrategy strategy)
    {
        _recipes = recipes;
        _source = source;
        _destination = destination;
        _strategy = strategy;
    }
    
    // Initialize the object 
    public void Initialize(
        List<Recipe> recipes,
        Container source,
        Container destination,
        IRecipeStrategy strategy)
    {
        _recipes = recipes;
        _source = source;
        _destination = destination;
        _strategy = strategy;
    }
    
    // -----------------------------
    // Getters
    // -----------------------------
    // Get destination (destination container)
    public Container? GetDestination()
    {
        return _destination;
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
    public Crafter Clone(IReadOnlyDictionary<Container, Container> containerMap)
    {
        List<Recipe> newRecipes = new List<Recipe>();
        foreach (Recipe recipe in _recipes)
        {
            newRecipes.Add(recipe.Clone());
        }
        
        return new Crafter(newRecipes,containerMap[_source], containerMap[_destination], _strategy.Clone());
    }
    
   
    // -----------------------------
    // Updating Crafter
    // -----------------------------
    public void Advance()
    {
        // Not currently crafting
        if (_currentRecipe == null)
        {
            Recipe? selected = _strategy.SelectRecipe(_recipes, _source);

            if (selected == null) { return;}

            if (!ConsumeInputs(selected)) { return; }
            _currentRecipe = selected;
            _progress = 0;
        }

        _progress++;

        if (_progress >= _currentRecipe.CraftTime)
        {
            foreach (Item output in _currentRecipe.Outputs)
            {
                _destination.AddItem(output.Name, output.Quantity);
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
    
    // Prints a summary of crafter
    public void Print()
    {
        int i = 0;

        foreach (Recipe recipe in _recipes)
        {
            Console.WriteLine("-----------------------------");

            if (recipe == _currentRecipe)
            {
                Console.WriteLine($"     *** Current Recipe *** ({i})");
            }
            else
            {
                Console.WriteLine($"     Recipe {i}");
            }

            // Print inputs
            Console.WriteLine("\n     Recipe Requires:");

            foreach (Item input in recipe.Inputs)
            {
                Console.WriteLine(
                    $"     Name: {input.Name}, Quantity: {input.Quantity}");
            }

            // Print outputs
            Console.WriteLine("\n     Recipe Outputs:");

            foreach (Item output in recipe.Outputs)
            {
                Console.WriteLine(
                    $"     Name: {output.Name}, Quantity: {output.Quantity}");
            }

            Console.WriteLine("-----------------------------");

            i++;
        }

        // If the crafter IS NOT currently crafting
        if (_currentRecipe == null)
        {
            Console.WriteLine("\nCrafter is idle.");
        }
        // If the crafter IS currently crafting 
        else
        {
            Console.WriteLine($"\nCrafting in progress: ({_progress}/{_currentRecipe.CraftTime})");
        }
    }

}


    
