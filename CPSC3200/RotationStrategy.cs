namespace CPSC3200;

public class RotationStrategy : IRecipeStrategy
{
    private int _currentIndex;
    public RotationStrategy(int currentIndex = 0)
    {
        _currentIndex = currentIndex;
    }
    public Recipe? SelectRecipe(
        List<Recipe> recipes,
        Container source)
    {
        if (recipes.Count == 0)
        {
            return null;
        }

        Recipe selected =
            recipes[_currentIndex];

        _currentIndex =
            (_currentIndex + 1) % recipes.Count;

        return selected;
    }

    public IRecipeStrategy Clone()
    {
        return new RotationStrategy
        {
            _currentIndex = _currentIndex
        };
    }
    public int GetCurrentIndex()
    {
        return _currentIndex;
    }
}

