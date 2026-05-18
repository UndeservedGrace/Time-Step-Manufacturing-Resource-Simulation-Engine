using System;
using System.Collections.Generic;

namespace CPSC3200
{
    public class Program
    {
        // -----------------------------
        // Command Constants
        // -----------------------------
        private const string CMD_EXIT  = "EXIT";
        private const string CMD_STEP  = "STEP";
        private const string CMD_DUPE  = "DUPE";
        private const string CMD_PRINT = "PRINT";
        private const string CMD_LIST  = "LS";
        private const string CMD_SET_RECIPE = "SETRECIPE";
        private const string CMD_PRINT_CRAFTERS = "CRAFTERS";

        // -----------------------------
        // Functions
        // -----------------------------
        
        // Split Commands
        static List<string> SplitString(string input, char delimiter)
        {
            return new List<string>(
                input.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
            );
        }

        // Print Summary
        static void PrintCommandList()
        {
            Console.WriteLine("\nAvailable Commands:");
            Console.WriteLine("EXIT");
            Console.WriteLine("STEP <sim_name> <steps>");
            Console.WriteLine("DUPE <sim_name> <new_name>");
            Console.WriteLine("PRINT <sim_name>");
            Console.WriteLine("LS (list simulations)");
            Console.WriteLine("SETRECIPE <sim_name> <crafter_index> <recipe_index>");
            Console.WriteLine("CRAFTERS <sim_name>");
            Console.WriteLine();
        }

        // -----------------------------
        // Main
        // -----------------------------
        public static void Main(string[] args)
        {
            // Map of all simulations
            Dictionary<string, Simulation> simulations =
                new Dictionary<string, Simulation>();

            // ----------------------------------------------------------
            // Test 1: Source == Destination
            // ----------------------------------------------------------

            simulations["TestSrcEqualsDestination"] = new Simulation();

            Simulation sim1 = simulations["TestSrcEqualsDestination"];

            Dictionary<string, Item> itemsTSED =
                new Dictionary<string, Item>() { { "Jamal", new Item("Jamal", 20) }, { "Tim", new Item("Tim", 20) } };

            // Source and destination are the same container
            Container sourceTSED = sim1.CreateContainer(itemsTSED);
            Container destTSED = sourceTSED;

            Recipe abominationRecipe = new Recipe(
                new List<Item>()
                { 
                    new Item("Jamal", 1),
                    new Item("Tim", 1)
                },
                new List<Item>()
                {
                    new Item("Abomination", 1)
                },
                2
            );

            sim1.CreateCrafter(
                new List<Recipe>() { abominationRecipe },
                destTSED,
                sourceTSED,
                new SingleActiveStrategy()
            );

            // ----------------------------------------------------------
            // Test 2: Basic Crafting
            // ----------------------------------------------------------

            simulations["TestBasicCraftingFunctionality"] = new Simulation();

            Simulation sim2 = simulations["TestBasicCraftingFunctionality"];

            Dictionary<string, Item> itemsTBCF =
                new Dictionary<string, Item>()
                {
                    { "h", new Item("h", 200) },
                    { "o", new Item("o", 100) }
                };

            Container sourceTBCF = sim2.CreateContainer(itemsTBCF);
            Container destTBCF = sim2.CreateContainer(new Dictionary<string, Item>());

            Recipe waterRecipe = new Recipe(
                new List<Item>()
                {
                    new Item("h", 2),
                    new Item("o", 1)
                },
                new List<Item>()
                {
                    new Item("water", 1)
                },
                1
            );

            sim2.CreateCrafter(new List<Recipe>() { waterRecipe }, destTBCF, sourceTBCF, new SingleActiveStrategy());

            // ----------------------------------------------------------
            // Test 3: One Inventory Shared Between Multiple Crafters
            // ----------------------------------------------------------

            simulations["TestOneInventoryLinkingToMultipleCrafters"] = new Simulation();

            Simulation sim3 = simulations["TestOneInventoryLinkingToMultipleCrafters"];

            Dictionary<string, Item> itemsTOILTMC =
                new Dictionary<string, Item>()
                {
                    { "flour", new Item("flour", 200) },
                    { "sugar", new Item("sugar", 200) },
                    { "egg", new Item("egg", 144) },
                    { "butter", new Item("butter", 64) },
                    { "milk", new Item("milk", 100) },
                    { "chocolate", new Item("chocolate", 50) }
                };

            Container sharedSource =
                sim3.CreateContainer(itemsTOILTMC);

            Container cookieDestination =
                sim3.CreateContainer(
                    new Dictionary<string, Item>()
                );

            Container omeletteDestination =
                sim3.CreateContainer(
                    new Dictionary<string, Item>()
                );

            Recipe cookieRecipe = new Recipe(
                new List<Item>()
                {
                    new Item("flour", 2),
                    new Item("sugar", 3),
                    new Item("egg", 3),
                    new Item("butter", 2)
                },
                new List<Item>()
                {
                    new Item("cookie", 1)
                },
                1
            );

            Recipe omeletteRecipe = new Recipe(
                new List<Item>()
                {
                    new Item("milk", 1),
                    new Item("egg", 3),
                    new Item("butter", 1)
                },
                new List<Item>()
                {
                    new Item("omelette", 1)
                },
                2
            );

            sim3.CreateCrafter(
                new List<Recipe>() { omeletteRecipe },
                omeletteDestination,
                sharedSource,
                new SingleActiveStrategy()
            );

            sim3.CreateCrafter(
                new List<Recipe>() { cookieRecipe },
                cookieDestination,
                sharedSource,
                new SingleActiveStrategy()
            );

            // ----------------------------------------------------------
            // Test 4: Crafters In Series
            // ----------------------------------------------------------
            simulations["TestCraftersInSeries"] = new Simulation();

            Simulation sim4 = simulations["TestCraftersInSeries"];

            Dictionary<string, Item> items1TCIS =
                new Dictionary<string, Item>()
                {
                    { "n2", new Item("n2", 8000) },
                    { "ch4", new Item("ch4", 1000) },
                    { "o2", new Item("o2", 2000) }
                };

            Dictionary<string, Item> items2TCIS =
                new Dictionary<string, Item>()
                {
                    { "water", new Item("water", 1000000) }
                };

            Container inventory1TCIS = sim4.CreateContainer(items1TCIS);

            Container inventory2TCIS = sim4.CreateContainer(items2TCIS);

            Recipe waterBreakdownRecipe = new Recipe(new List<Item>()
                {
                    new Item("water", 100)
                },
                new List<Item>()
                {
                    new Item("h2", 200),
                    new Item("o2", 100)
                },
                1
            );

            Recipe combustionRecipe = new Recipe(
                new List<Item>()
                {
                    new Item("ch4", 10),
                    new Item("o2", 20)
                },
                new List<Item>()
                {
                    new Item("water", 20),
                    new Item("co2", 10)
                },
                1
            );

            sim4.CreateCrafter(
                new List<Recipe>() { waterBreakdownRecipe },
                inventory1TCIS,
                inventory2TCIS,
                new SingleActiveStrategy()
            );

            sim4.CreateCrafter(
                new List<Recipe>() { combustionRecipe },
                inventory2TCIS,
                inventory1TCIS,
                new SingleActiveStrategy()
            );
            // ----------------------------------------------------------
            // Test 5: Single Active Recipe Demonstration
            // ----------------------------------------------------------
            simulations["SingleActiveDemo"] = new Simulation();

            Simulation singleSim = simulations["SingleActiveDemo"];

            Container singleSource =
                singleSim.CreateContainer(
                    new Dictionary<string, Item>()
                    {
                        { "wood", new Item("wood", 20) },
                        { "stone", new Item("stone", 20) },
                        { "iron", new Item("iron", 20) }
                    });

            Container singleDest =
                singleSim.CreateContainer(
                    new Dictionary<string, Item>());

            Recipe axeRecipe = new Recipe(
                new List<Item>()
                {
                    new Item("wood", 2),
                    new Item("stone", 1)
                },
                new List<Item>()
                {
                    new Item("axe", 1)
                },
                2);

            Recipe swordRecipe = new Recipe(
                new List<Item>()
                {
                    new Item("wood", 1),
                    new Item("iron", 3)
                },
                new List<Item>()
                {
                    new Item("sword", 1)
                },
                3);

            singleSim.CreateCrafter(
                new List<Recipe>()
                {
                    axeRecipe,
                    swordRecipe
                },
                singleDest,
                singleSource,
                new SingleActiveStrategy());
            // ----------------------------------------------------------
            // Test 6: PriorityStrategy Demonstration
            // ----------------------------------------------------------
            simulations["PriorityDemo"] =
                new Simulation();

            Simulation prioritySim =
                simulations["PriorityDemo"];

            Container prioritySource =
                prioritySim.CreateContainer(
                    new Dictionary<string, Item>()
                    {
                        { "wood", new Item("wood", 20) },
                        { "stone", new Item("stone", 2) },
                        { "iron", new Item("iron", 10) }
                    });

            Container priorityDest =
                prioritySim.CreateContainer(
                    new Dictionary<string, Item>());

            Recipe cheapTool = new Recipe(
                new List<Item>()
                {
                    new Item("wood", 1),
                    new Item("stone", 1)
                },
                new List<Item>()
                {
                    new Item("stone_tool", 1)
                },
                1);

            Recipe expensiveTool = new Recipe(
                new List<Item>()
                {
                    new Item("wood", 1),
                    new Item("iron", 3)
                },
                new List<Item>()
                {
                    new Item("iron_tool", 1)
                },
                1);

            prioritySim.CreateCrafter(
                new List<Recipe>()
                {
                    expensiveTool,
                    cheapTool
                },
                priorityDest,
                prioritySource,
                new PriorityStrategy());
            
            // ----------------------------------------------------------
            // Test 7: Rotation Demonstration
            // ----------------------------------------------------------
            simulations["RotationDemo"] =
                new Simulation();

            Simulation rotationSim =
                simulations["RotationDemo"];

            Container rotationSource =
                rotationSim.CreateContainer(
                    new Dictionary<string, Item>()
                    {
                        { "a", new Item("a", 20) },
                        { "b", new Item("b", 20) },
                        { "c", new Item("c", 20) }
                    });

            Container rotationDest =
                rotationSim.CreateContainer(
                    new Dictionary<string, Item>());

            Recipe recipeA = new Recipe(
                new List<Item>()
                {
                    new Item("a", 1)
                },
                new List<Item>()
                {
                    new Item("productA", 1)
                },
                1);

            Recipe recipeB = new Recipe(
                new List<Item>()
                {
                    new Item("b", 1)
                },
                new List<Item>()
                {
                    new Item("productB", 1)
                },
                1);

            Recipe recipeC = new Recipe(
                new List<Item>()
                {
                    new Item("c", 1)
                },
                new List<Item>()
                {
                    new Item("productC", 1)
                },
                1);

            rotationSim.CreateCrafter(
                new List<Recipe>()
                {
                    recipeA,
                    recipeB,
                    recipeC
                },
                rotationDest,
                rotationSource,
                new RotationStrategy());
            
            // ----------------------------------------------------------
            // Command Loop
            // ----------------------------------------------------------
            PrintCommandList();

            bool running = true;

            while (running)
            {
                Console.Write("Enter command: ");

                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                List<string> commands = SplitString(input, ' ');

                // ---------------- EXIT ----------------

                if (commands[0] == CMD_EXIT)
                {
                    running = false;
                }
                // ---------------- PRINT_CRAFTERS ----------------
                else if (commands[0] == CMD_PRINT_CRAFTERS)
                {
                    if (commands.Count < 2)
                    {
                        Console.WriteLine("Usage: CRAFTERS <sim_name>");
                    }
                    else
                    {
                        if (!simulations.TryGetValue(commands[1], out Simulation? simulation))
                        {
                            Console.WriteLine("Simulation not found");
                        }
                        else
                        {
                            simulation.PrintCrafters();
                        }
                    }

                }
                // ---------------- STEP ----------------
                
                else if (commands[0] == CMD_SET_RECIPE)
                {
                    if (commands.Count < 4)
                    {
                        Console.WriteLine(
                            "Usage: SETRECIPE <sim_name> <crafter_index> <recipe_index>");
                        continue;
                    }

                    if (!simulations.TryGetValue(
                            commands[1],
                            out Simulation? simulation))
                    {
                        Console.WriteLine("Simulation not found");
                        continue;
                    }

                    try
                    {
                        int crafterIndex = int.Parse(commands[2]);
                        int recipeIndex = int.Parse(commands[3]);

                        Crafter? crafter =
                            simulation.GetCrafter(crafterIndex);

                        if (crafter == null)
                        {
                            Console.WriteLine("Invalid crafter index");
                            continue;
                        }

                        if (crafter.GetStrategy()
                            is SingleActiveStrategy strategy)
                        {
                            strategy.SetActiveRecipe(recipeIndex);

                            Console.WriteLine(
                                $"Crafter {crafterIndex} active recipe set to {recipeIndex}");
                        }
                        else
                        {
                            Console.WriteLine(
                                "Crafter is not using SingleActiveStrategy");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Invalid indexes");
                    }
                }

                else if (commands[0] == CMD_STEP)
                {
                    if (commands.Count < 3)
                    {
                        Console.WriteLine(
                            "Usage: STEP <sim_name> <steps>"
                        );
                        continue;
                    }

                    if (!simulations.TryGetValue(
                            commands[1],
                            out Simulation? simulation))
                    {
                        Console.WriteLine(
                            "Simulation not found"
                        );
                        continue;
                    }

                    try
                    {
                        int steps = int.Parse(commands[2]);

                        if (steps <= 0)
                        {
                            Console.WriteLine(
                                "Steps must be positive"
                            );
                            continue;
                        }

                        for (int i = 0; i < steps; i++)
                        {
                            simulation.Step();
                        }
                    }
                    catch
                    {
                        Console.WriteLine(
                            "Invalid step count"
                        );
                    }
                }

                // ---------------- PRINT ----------------

                else if (commands[0] == CMD_PRINT)
                {
                    if (commands.Count < 2)
                    {
                        Console.WriteLine(
                            "Usage: PRINT <sim_name>"
                        );
                        continue;
                    }

                    if (!simulations.TryGetValue(
                            commands[1],
                            out Simulation? simulation))
                    {
                        Console.WriteLine(
                            "Simulation not found"
                        );
                        continue;
                    }

                    simulation.Print();
                }

                // ---------------- LIST ----------------

                else if (commands[0] == CMD_LIST)
                {
                    Console.WriteLine("Simulations:");

                    foreach (var pair in simulations)
                    {
                        Console.WriteLine(
                            $" - {pair.Key}"
                        );
                    }
                }

                // ---------------- DUPE ----------------

                else if (commands[0] == CMD_DUPE)
                {
                    if (commands.Count < 3)
                    {
                        Console.WriteLine(
                            "Usage: DUPE <sim_name> <new_name>"
                        );
                        continue;
                    }

                    if (!simulations.TryGetValue(
                            commands[1],
                            out Simulation? simulation))
                    {
                        Console.WriteLine(
                            "Simulation not found"
                        );
                        continue;
                    }

                    simulations[commands[2]] =
                        new Simulation(simulation);

                    Console.WriteLine(
                        $"Created simulation: {commands[2]}"
                    );
                }

                // ---------------- UNKNOWN ----------------

                else
                {
                    Console.WriteLine("Unknown command");
                    PrintCommandList();
                }
            }
        }
    }
}