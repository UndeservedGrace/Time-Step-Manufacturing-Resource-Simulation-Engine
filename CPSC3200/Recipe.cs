namespace CPSC3200;

public class Recipe
{
    public List<Item> Inputs { get; }
    public List<Item> Outputs { get; }
    public int CraftTime { get; }

    public Recipe(
        List<Item> inputs,
        List<Item> outputs,
        int craftTime)
    {
        Inputs = inputs;
        Outputs = outputs;
        CraftTime = craftTime;
    }

    public Recipe Clone()
    {
        List<Item> newInputs = new List<Item>();
        List<Item> newOutputs = new List<Item>();
        
        foreach (Item i in Inputs)
        {
            newInputs.Add(new Item(i.Name, i.Quantity));
        }
        
        foreach (Item i in Outputs)
        {
            newOutputs.Add(new Item(i.Name, i.Quantity));
        }
        
        return new Recipe(newInputs, newOutputs, CraftTime);
    }
}