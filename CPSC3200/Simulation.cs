using System.Numerics;

namespace CPSC3200;
using System.Collections.Generic;

public class Simulation
{
    
    List<Crafter> _crafters = new List<Crafter>();
    List<Container> _containers = new List<Container>();

    public Simulation(){}
        /// Creates a container and stores it in the simulation.
        public Container CreateContainer(Dictionary<string, Item> items)
        {
            Container newContainer = new Container(items);
            _containers.Add(newContainer);
            return newContainer;
        }
        
        // Prints the contents of the crafters
        public void PrintCrafters()
        {
            for (int i = 0; i < _crafters.Count; i++)
            {
                Console.WriteLine($"Crafter {i}");
                _crafters[i].Print();
                Console.WriteLine();
            }
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
       
        public Crafter CreateCrafter(List<Recipe> recipes, Container destination, Container source, IRecipeStrategy strategy)
        {
            // Pass source to source, and destination to destination!
            Crafter newCrafter = new Crafter(recipes, source, destination, strategy);

            _crafters.Add(newCrafter);
            return newCrafter;
        }

        /// Advances all crafters by one simulation tick.
        public void Step()
        {
            foreach (Crafter crafter in _crafters)
            {
                crafter.Advance();
            }
        }
        
        // Printing

        /// Prints all containers and crafters.
        public void Print()
        {
            Console.WriteLine("\n-----------------------------");
            Console.WriteLine("Simulation State");
            Console.WriteLine("-----------------------------");

            // Print Containers
            for (int i = 0; i < _containers.Count; i++)
            {
                Console.WriteLine($"\nContainer {i}:");

                _containers[i].Print();
            }

            // Print Crafters
            for (int i = 0; i < _crafters.Count; i++)
            {
                Console.WriteLine($"\nCrafter {i}:");

                _crafters[i].Print();
            }

            Console.WriteLine();
        }
    
    public Simulation(Simulation other)
    {
        Dictionary<Container, Container> containerMap = new();
    
        foreach (Container oldContainer in other._containers)
        {
            Container newContainer = oldContainer.Clone();
            _containers.Add(newContainer);
            
            containerMap[oldContainer] = newContainer;
        }
        Dictionary<Crafter, Crafter> crafterMap = new();
        
        foreach (Crafter oldCrafter in other._crafters)
        {
            Crafter newCrafter = new Crafter(new List<Recipe>(), null, null, null);

            _crafters.Add(newCrafter);

            crafterMap[oldCrafter] = newCrafter;
        }
        
        for (int i = 0; i < other._crafters.Count; i++)
        {
            Crafter oldCrafter = other._crafters[i];
            Crafter newCrafter = _crafters[i];

            List<Recipe> recipes = new();

            foreach (Recipe recipe in oldCrafter.GetRecipes())
            {
                recipes.Add(recipe.Clone());
            }

            Container newSource =
                containerMap[oldCrafter.GetSource()];

            Container newDestination =
                containerMap[oldCrafter.GetDestination()];

            IRecipeStrategy strategy =
                oldCrafter
                    .GetStrategy()
                    .Clone();

            newCrafter.Initialize(
                recipes,
                newSource,
                newDestination,
                strategy);
        }
    }
}