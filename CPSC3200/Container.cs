namespace CPSC3200;

public class Container
{
    // Stores all items by item name
    private readonly Dictionary<string, Item> _items;

    // Constructor
    public Container()
    {
        _items = new Dictionary<string, Item>();
    }

    // Overloaded Constructor
    public Container(Dictionary<string, Item> items)
    {
        _items = new Dictionary<string, Item>();
        foreach (var pair in items)
        {
            _items[pair.Key] = new Item(pair.Value.Name, pair.Value.Quantity);
        }
    }

    // -----------------------------
    // Item Management
    // -----------------------------

    // Adds quantity to an item
    // Creates item if it does not exist
    public void AddItem(string name, int quantity)
    {
        if (quantity <= 0) { return; }

        if (_items.TryGetValue(name, out Item? existing))
        {
            _items[name] = existing with { Quantity = existing.Quantity + quantity };
        }
        else
        {
            _items[name] = new Item(name, quantity);
        }
    }

    public void AddItem(Item item)
    {
        AddItem(item.Name, item.Quantity);
    }

    // Returns true if container has enough quantity to craft
    public bool HasItem(string name, int quantity)
    {
        if (quantity <= 0) { return false; }
        
        if (_items.TryGetValue(name, out Item? item))
        {
            return item.Quantity >= quantity;
        }

        return false;
    }

    // Removes quantity if enough exists
    public bool RemoveItem(string name, int quantity)
    {
        if (!HasItem(name, quantity))
        {
            return false;
        }

        Item current = _items[name];

        int remaining = current.Quantity - quantity;

        // Remove empty stacks
        if (remaining <= 0)
        {
            _items.Remove(name);
        }
        else
        {
            _items[name] = current with { Quantity = remaining };
        }

        return true;
    }

    // -----------------------------
    // Other
    // -----------------------------

    // Returns a deep copy of all items
    public Dictionary<string, Item> GetItems()
    {
        Dictionary<string, Item> copy =
            new Dictionary<string, Item>();

        foreach (var pair in _items)
        {
            copy[pair.Key] = new Item(pair.Value.Name, pair.Value.Quantity);
        }
        
        return copy;
    }
    
    // Creates a deep copy of the container
    public Container Clone()
    {
        return new Container(GetItems());
    }

    // Prints contents
    public void Print()
    {
        Console.WriteLine("Container Contents:");

        if (_items.Count == 0)
        {
            Console.WriteLine("(empty)");
            return;
        }

        foreach (var pair in _items)
        {
            Console.WriteLine($"Item: {pair.Value.Name}, " + $"Quantity: {pair.Value.Quantity}");
        }
    }
}