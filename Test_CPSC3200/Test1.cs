
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CPSC3200;
using System.Collections.Generic;

namespace CPSC3200
{
    // CONTAINER TESTS
    [TestClass]
    public class ContainerTests
    {
        [TestMethod]
        public void Container_AddItem_InvalidQuantity_ReturnsEarly()
        {
            Container container = new Container();
            container.AddItem("wood", 0);
            // Entering an invalid quantity
            container.AddItem("stone", -5);

            Assert.IsFalse(container.HasItem("wood", 1));
            Assert.IsFalse(container.HasItem("stone", 1));
        }

        [TestMethod]
        public void Container_AddItem_UsingItemRecord_AddsSuccessfully()
        {
            Container container = new Container();
            Item item = new Item("iron", 10);

            container.AddItem(item);

            Assert.IsTrue(container.HasItem("iron", 10));
        }

        [TestMethod]
        public void Container_HasItem_InvalidQuantityOrMissingKey_ReturnsFalse()
        {
            Container container = new Container();
            container.AddItem("wood", 10);

            Assert.IsFalse(container.HasItem("wood", 0));
            Assert.IsFalse(container.HasItem("wood", -5));
            Assert.IsFalse(container.HasItem("diamond", 1));
        }

        [TestMethod]
        public void Container_RemoveItem_DropsToZero_RemovesKeyFromDictionary()
        {
            Container container = new Container();
            container.AddItem("wood", 5);

            // Removing exactly the total quantity should remove the key entirely
            bool result = container.RemoveItem("wood", 5);

            Assert.IsTrue(result);
            Assert.IsFalse(container.HasItem("wood", 1));
            
            Assert.AreEqual(0, container.GetItems().Count);
        }

        [TestMethod]
        public void Container_Print_HandlesEmptyAndPopulated()
        {
            Container container = new Container();
            
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                
                container.Print();
                string emptyOutput = sw.ToString();
                Assert.IsTrue(emptyOutput.Contains("(empty)"));

                // Reset and print with items
                sw.GetStringBuilder().Clear();
                container.AddItem("coal", 3);
                container.Print();
                string populatedOutput = sw.ToString();
                Assert.IsTrue(populatedOutput.Contains("Item: coal, Quantity: 3"));
            }
        }
        
        [TestMethod]
        public void AddItem_NewItem_AddsSuccessfully()
        {
            Container container = new Container();

            container.AddItem("wood", 5);

            Assert.IsTrue(container.HasItem("wood", 5));
        }

        [TestMethod]
        public void AddItem_ExistingItem_IncreasesQuantity()
        {
            Container container = new Container();

            container.AddItem("wood", 5);
            container.AddItem("wood", 3);

            Assert.IsTrue(container.HasItem("wood", 8));
        }

        [TestMethod]
        public void RemoveItem_EnoughQuantity_RemovesSuccessfully()
        {
            Container container = new Container();

            container.AddItem("stone", 10);

            bool result = container.RemoveItem("stone", 4);

            Assert.IsTrue(result);
            Assert.IsTrue(container.HasItem("stone", 6));
        }

        [TestMethod]
        public void RemoveItem_NotEnoughQuantity_ReturnsFalse()
        {
            Container container = new Container();

            container.AddItem("stone", 2);

            bool result = container.RemoveItem("stone", 5);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Clone_ProducesDeepCopy()
        {
            Container original = new Container();

            original.AddItem("iron", 10);

            Container clone = original.Clone();

            clone.AddItem("iron", 5);

            Assert.IsTrue(original.HasItem("iron", 10));
            Assert.IsTrue(clone.HasItem("iron", 15));
        }
    }

    // RECIPE TESTS
    [TestClass]
    public class RecipeTests
    {
        [TestMethod]
        public void Clone_ProducesDeepCopy()
        {
            Recipe recipe = new Recipe(
                new List<Item>()
                {
                    new Item("wood", 2)
                },
                new List<Item>()
                {
                    new Item("plank", 4)
                },
                3);

            Recipe clone = recipe.Clone();

            Assert.AreNotSame(recipe, clone);
            Assert.AreEqual(recipe.CraftTime, clone.CraftTime);
            Assert.AreEqual(
                recipe.Inputs[0].Name,
                clone.Inputs[0].Name);
        }
    }
    
    // STRATEGY TESTS
    [TestClass]
    public class StrategyTests
    {
        [TestMethod]
        public void PriorityStrategy_NoValidRecipes_ReturnsNull()
        {
            PriorityStrategy strategy = new PriorityStrategy();
            Container source = new Container();
            List<Recipe> recipes = new List<Recipe> { new Recipe(new List<Item> { new Item("mana", 10) }, new List<Item>(), 1) };

            Recipe result = strategy.SelectRecipe(recipes, source);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void RotationStrategy_EmptyRecipeList_ReturnsNull()
        {
            RotationStrategy strategy = new RotationStrategy();
            Container source = new Container();
            
            var result = strategy.SelectRecipe(new List<Recipe>(), source);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SingleActiveStrategy_IndexOutOfBounds_ReturnsNull()
        {
            // Index 5 doesn't exist
            var strategy = new SingleActiveStrategy(5); 
            var source = new Container();
            var recipes = new List<Recipe> { new Recipe(new List<Item>(), new List<Item>(), 1) };

            var result = strategy.SelectRecipe(recipes, source);
            Assert.IsNull(result);

            strategy.SetActiveRecipe(-1);
            result = strategy.SelectRecipe(recipes, source);
            Assert.IsNull(result);
        }
        
        [TestMethod]
        public void PriorityStrategy_SelectsFirstCraftableRecipe()
        {
            Container source = new Container();

            source.AddItem("wood", 10);

            Recipe impossibleRecipe = new Recipe(
                new List<Item>()
                {
                    new Item("iron", 5)
                },
                new List<Item>()
                {
                    new Item("sword", 1)
                },
                1);

            Recipe validRecipe = new Recipe(
                new List<Item>()
                {
                    new Item("wood", 2)
                },
                new List<Item>()
                {
                    new Item("plank", 1)
                },
                1);

            PriorityStrategy strategy =
                new PriorityStrategy();

            Recipe? selected =
                strategy.SelectRecipe(
                    new List<Recipe>()
                    {
                        impossibleRecipe,
                        validRecipe
                    },
                    source);

            Assert.AreEqual(validRecipe, selected);
        }

        [TestMethod]
        public void RotationStrategy_RotatesRecipes()
        {
            Container source = new Container();

            Recipe recipe1 = new Recipe(
                new List<Item>(),
                new List<Item>()
                {
                    new Item("A", 1)
                },
                1);

            Recipe recipe2 = new Recipe(
                new List<Item>(),
                new List<Item>()
                {
                    new Item("B", 1)
                },
                1);

            RotationStrategy strategy =
                new RotationStrategy();

            List<Recipe> recipes =
                new List<Recipe>()
                {
                    recipe1,
                    recipe2
                };

            Recipe? first =
                strategy.SelectRecipe(recipes, source);

            Recipe? second =
                strategy.SelectRecipe(recipes, source);

            Assert.AreEqual(recipe1, first);
            Assert.AreEqual(recipe2, second);
        }

        [TestMethod]
        public void SingleActiveStrategy_SelectsCorrectRecipe()
        {
            Container source = new Container();

            Recipe recipe1 = new Recipe(
                new List<Item>(),
                new List<Item>()
                {
                    new Item("A", 1)
                },
                1);

            Recipe recipe2 = new Recipe(
                new List<Item>(),
                new List<Item>()
                {
                    new Item("B", 1)
                },
                1);

            SingleActiveStrategy strategy =
                new SingleActiveStrategy(1);

            Recipe? selected =
                strategy.SelectRecipe(
                    new List<Recipe>()
                    {
                        recipe1,
                        recipe2
                    },
                    source);

            Assert.AreEqual(recipe2, selected);
        }

        [TestMethod]
        public void PriorityStrategy_ClonesMakesDeepCopy()
        {
            IRecipeStrategy strategy1 = new PriorityStrategy();
            IRecipeStrategy strategy2 = strategy1.Clone();
            Assert.AreNotSame(strategy1, strategy2);
        }

    }

   [TestClass]
    public class CrafterTests
    {
        [TestMethod]
        public void Crafter_Initialize_ReassignsInternalDependencies()
        {
            Container source1 = new Container();
            Container dest1 = new Container();
            PriorityStrategy strategy1 = new PriorityStrategy();
            List<Recipe> recipes1 = new List<Recipe>();

            Crafter crafter = new Crafter(recipes1, source1, dest1, strategy1);

            Container source2 = new Container();
            Container dest2 = new Container();
            RotationStrategy strategy2 = new RotationStrategy();
            List<Recipe> recipes2 = new List<Recipe> { new Recipe(new List<Item>(), new List<Item>(), 1) };

            crafter.Initialize(recipes2, source2, dest2, strategy2);

            Assert.AreSame(source2, crafter.GetSource());
            Assert.AreSame(dest2, crafter.GetDestination());
            Assert.AreSame(strategy2, crafter.GetStrategy());
            Assert.AreEqual(1, crafter.GetRecipes().Count);
        }

        [TestMethod]
        public void Crafter_Advance_NullSelectedRecipe_ReturnsEarly()
        {
            Container source = new Container();
            Container dest = new Container();
            // Priority Strategy returns null if inputs aren't met
            Recipe recipe = new Recipe(new List<Item> { new Item("wood", 1) }, new List<Item> { new Item("plank", 1) }, 1);
            Crafter crafter = new Crafter(new List<Recipe> { recipe }, source, dest, new PriorityStrategy());

            crafter.Advance();

            Assert.IsNull(crafter.GetCurrentRecipe());
        }

        [TestMethod]
        public void Crafter_Advance_NullSourceContainer_FailsConsumeInputs()
        {
            Container dest = new Container();
            Recipe recipe = new Recipe(new List<Item> { new Item("wood", 1) }, new List<Item> { new Item("plank", 1) }, 1);
            // Intentionally passing null for source container
            Crafter crafter = new Crafter(new List<Recipe> { recipe }, null, dest, new SingleActiveStrategy(0));

            crafter.Advance();

            Assert.IsNull(crafter.GetCurrentRecipe());
        }

        [TestMethod]
        public void Crafter_Advance_InsufficientResources_FailsConsumeInputs()
        {
            Container source = new Container();
            source.AddItem("wood", 2);
            Container dest = new Container();
            Recipe recipe = new Recipe(new List<Item> { new Item("wood", 5) }, new List<Item> { new Item("plank", 1) }, 1);
            Crafter crafter = new Crafter(new List<Recipe> { recipe }, source, dest, new SingleActiveStrategy(0));

            crafter.Advance();

            Assert.IsNull(crafter.GetCurrentRecipe());
            Assert.AreEqual(2, source.GetItems()["wood"].Quantity); 
        }

        [TestMethod]
        public void Crafter_Advance_MultiTickCraftingProgress()
        {
            Container source = new Container();
            Container dest = new Container();
            Recipe recipe = new Recipe(new List<Item>(), new List<Item> { new Item("gold", 1) }, 3); 
            Crafter crafter = new Crafter(new List<Recipe> { recipe }, source, dest, new SingleActiveStrategy(0));

            // Time 1: Starts crafting, processes progress to 1
            crafter.Advance();
            Assert.IsNotNull(crafter.CurrentRecipe);
            Assert.IsFalse(dest.HasItem("gold", 1));

            // Time 2: Progresses to 2
            crafter.Advance();
            Assert.IsFalse(dest.HasItem("gold", 1));

            // Time 3: Reaches progress 3 -> Completes and resets
            crafter.Advance();
            Assert.IsNull(crafter.CurrentRecipe);
            Assert.IsTrue(dest.HasItem("gold", 1));
        }

        [TestMethod]
        public void Crafter_Print_OutputCoverage()
        {
            Container source = new Container();
            Container dest = new Container();
            Recipe recipe = new Recipe(new List<Item> { new Item("iron", 1) }, new List<Item> { new Item("bar", 1) }, 2);
            Crafter crafter = new Crafter(new List<Recipe> { recipe }, source, dest, new SingleActiveStrategy(0));

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Idle print check
                crafter.Print();
                string idleText = sw.ToString();
                Assert.IsTrue(idleText.Contains("Crafter is idle."));

                // Active print check
                source.AddItem("iron", 1);
                crafter.Advance(); // Puts it into progress state
                sw.GetStringBuilder().Clear();
                
                crafter.Print();
                string activeText = sw.ToString();
                Assert.IsTrue(activeText.Contains("*** Current Recipe ***"));
                Assert.IsTrue(activeText.Contains("Crafting in progress: (1/2)"));
            }
        }
        
        [TestMethod]
        public void Advance_CompletesCraft_AddsOutput()
        {
            Container source = new Container();
            Container destination = new Container();

            source.AddItem("wood", 5);

            Recipe recipe = new Recipe(
                new List<Item>()
                {
                    new Item("wood", 2)
                },
                new List<Item>()
                {
                    new Item("plank", 1)
                },
                1);

            Crafter crafter = new Crafter(
                new List<Recipe>()
                {
                    recipe
                },
                source,
                destination,
                new SingleActiveStrategy());

            crafter.Advance();

            Assert.IsTrue(destination.HasItem("plank", 1));
        }

        [TestMethod]
        public void Advance_NotEnoughResources_DoesNothing()
        {
            Container source = new Container();
            Container destination = new Container();

            Recipe recipe = new Recipe(
                new List<Item>()
                {
                    new Item("wood", 5)
                },
                new List<Item>()
                {
                    new Item("plank", 1)
                },
                1);

            Crafter crafter = new Crafter(
                new List<Recipe>()
                {
                    recipe
                },
                source,
                destination,
                new SingleActiveStrategy());

            crafter.Advance();

            Assert.IsFalse(destination.HasItem("plank", 1));
        }

        [TestMethod]
        public void Clone_ProducesIndependentCrafter()
        {
            Container source = new Container();
            Container destination = new Container();

            source.AddItem("wood", 10);

            Recipe recipe = new Recipe(
                new List<Item>()
                {
                    new Item("wood", 2)
                },
                new List<Item>()
                {
                    new Item("plank", 1)
                },
                1);

            Crafter original = new Crafter(
                new List<Recipe>()
                {
                    recipe
                },
                source,
                destination,
                new SingleActiveStrategy());

            Dictionary<Container, Container> map =
                new Dictionary<Container, Container>()
                {
                    { source, source.Clone() },
                    { destination, destination.Clone() }
                };

            Crafter clone = original.Clone(map);

            Assert.AreNotSame(original, clone);
        }
    }

    // SIMULATION TESTS
    [TestClass]
    public class SimulationTests
    {
        [TestMethod]
        public void Simulation_GetCrafter_OutOfBounds_ReturnsNull()
        {
            Simulation sim = new Simulation();
            
            Assert.IsNull(sim.GetCrafter(-1));
            Assert.IsNull(sim.GetCrafter(0)); 
        }
        
        [TestMethod]
        public void Simulation_PrintAndPrintCrafters_Coverage()
        {
            Simulation sim = new Simulation();
            Container container = sim.CreateContainer(new Dictionary<string, Item>());
            sim.CreateCrafter(new List<Recipe>(), container, container, new PriorityStrategy());

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                sim.PrintCrafters();
                string craftersText = sw.ToString();
                Assert.IsTrue(craftersText.Contains("Crafter 0"));

                sw.GetStringBuilder().Clear();

                sim.Print();
                string globalText = sw.ToString();
                Assert.IsTrue(globalText.Contains("Simulation State"));
                Assert.IsTrue(globalText.Contains("Container 0:"));
            }
        }
        
        [TestMethod]
        public void Simulation_CopyConstructor_DeepCopiesDeepStateSystematically()
        {
            Simulation originalSim = new Simulation();
            Dictionary<string, Item> itemsDict = new Dictionary<string, Item> { { "ore", new Item("ore", 10) } };
            
            Container src = originalSim.CreateContainer(itemsDict);
            Container dest = originalSim.CreateContainer(new Dictionary<string, Item>());
            
            Recipe recipe = new Recipe(new List<Item> { new Item("ore", 2) }, new List<Item> { new Item("ingot", 1) }, 1);
            originalSim.CreateCrafter(new List<Recipe> { recipe }, dest, src, new RotationStrategy());

            Simulation clonedSim = new Simulation(originalSim);

            clonedSim.Step();

            Container cloneDest = clonedSim.GetCrafter(0).GetDestination();
            Container originalDest = originalSim.GetCrafter(0).GetDestination();

            Assert.IsTrue(cloneDest.HasItem("ingot", 1));
            Assert.IsFalse(originalDest.HasItem("ingot", 1));
        }
    
        [TestMethod]
        public void Step_AdvancesAllCrafters()
        {
            Simulation simulation = new Simulation();

            Container source =
                simulation.CreateContainer(
                    new Dictionary<string, Item>());

            Container destination =
                simulation.CreateContainer(
                    new Dictionary<string, Item>());

            source.AddItem("wood", 10);

            Recipe recipe = new Recipe(
                new List<Item>()
                {
                    new Item("wood", 2)
                },
                new List<Item>()
                {
                    new Item("plank", 1)
                },
                1);

            simulation.CreateCrafter(
                new List<Recipe>()
                {
                    recipe
                },
                destination,
                source,
                new SingleActiveStrategy());

            simulation.Step();

            Assert.IsTrue(destination.HasItem("plank", 1));
        }

        [TestMethod]
        public void CopyConstructor_ProducesDeepCopy()
        {
            Simulation original = new Simulation();

            Container source =
                original.CreateContainer(
                    new Dictionary<string, Item>());

            Container destination =
                original.CreateContainer(
                    new Dictionary<string, Item>());

            source.AddItem("wood", 10);

            Recipe recipe = new Recipe(
                new List<Item>()
                {
                    new Item("wood", 2)
                },
                new List<Item>()
                {
                    new Item("plank", 1)
                },
                1);

            original.CreateCrafter(
                new List<Recipe>()
                {
                    recipe
                },
                destination,
                source,
                new SingleActiveStrategy());

            Simulation copy =
                new Simulation(original);

            copy.Step();

            Crafter? originalCrafter =
                original.GetCrafter(0);

            Crafter? copiedCrafter =
                copy.GetCrafter(0);

            Assert.AreNotSame(originalCrafter, copiedCrafter);
        }
    }
}