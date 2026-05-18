namespace CPSC3200;

public interface IRecipeStrategy
{
    Recipe? SelectRecipe(
        List<Recipe> recipes,
        Container source);

    IRecipeStrategy Clone();
}