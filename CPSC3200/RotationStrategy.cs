namespace CPSC3200;

public class RotationStrategy : IRecipeStrategy
{
    private int _currentIndex = 0;

    public Recipe? SelectRecipe(List<Recipe> recipes, Container source)
    {
        if (recipes.Count == 0) { return null; }

        Recipe selected = recipes[_currentIndex];
        
        // Loops back around if reaches end
        _currentIndex = (_currentIndex + 1) % recipes.Count;
        return selected;
    }
    
    public IRecipeStrategy Clone()
    {
        return new RotationStrategy
        {
            _currentIndex = _currentIndex
        };
    }
}

