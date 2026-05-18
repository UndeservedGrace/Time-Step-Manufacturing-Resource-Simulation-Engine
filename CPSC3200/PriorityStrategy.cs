namespace CPSC3200;

public class PriorityStrategy : IRecipeStrategy
{
    public Recipe? SelectRecipe(
        List<Recipe> recipes,
        Container source)
    {
        foreach (Recipe recipe in recipes)
        {
            bool canCraft = true;

            foreach (Item input in recipe.Inputs)
            {
                if (!source.HasItem(input.Name, input.Quantity))
                {
                    canCraft = false;
                    break;
                }
            }

            if (canCraft)
            {
                return recipe;
            }
        }

        return null;
    }
    
    public IRecipeStrategy Clone()
    {
        return new PriorityStrategy();
    }
}