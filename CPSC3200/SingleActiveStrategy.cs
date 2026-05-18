namespace CPSC3200;

public class SingleActiveStrategy : IRecipeStrategy
{
    private int _activeRecipeIndex;

   public SingleActiveStrategy(int activeRecipeIndex = 0)
   { 
       _activeRecipeIndex = activeRecipeIndex;
   }

   public void SetActiveRecipe(int index)
   {
       _activeRecipeIndex = index;
   }

   public Recipe? SelectRecipe(List<Recipe> recipes, Container source)
   {
       if (_activeRecipeIndex < 0 || _activeRecipeIndex >= recipes.Count)
       {
           return null;
       }

       return recipes[_activeRecipeIndex];
   }

   public IRecipeStrategy Clone()
   {
       return new SingleActiveStrategy(_activeRecipeIndex);
   }
}