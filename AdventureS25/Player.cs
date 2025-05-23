
namespace AdventureS25;

public static class Player
{
    public static Location CurrentLocation;
    public static List<Item> Inventory;

    public static void Initialize()
    {
        Inventory = new List<Item>();
        CurrentLocation = Map.StartLocation;
    }

    public static void Move(Command command)
    {
        if (CurrentLocation.CanMoveInDirection(command))
        {
            CurrentLocation = CurrentLocation.GetLocationInDirection(command);
            Console.WriteLine(CurrentLocation.GetDescription());
        }
        else
        {
            Console.WriteLine("You can't move " + command.Noun + ".");
        }
    }

    public static string GetLocationDescription()
    {
        return CurrentLocation.GetDescription();
    }

    public static void Take(Command command)
    {
        // figure out which item to take: turn the noun into an item
        Item item = Items.GetItemByName(command.Noun);

        if (item == null)
        {
            Console.WriteLine("I don't know what " + command.Noun + " is.");
        }
        else if (!CurrentLocation.HasItem(item))
        {
            Console.WriteLine("There is no " + command.Noun + " here.");
        }
        else if (!item.IsTakeable)
        {
            Console.WriteLine("The " + command.Noun + " can't be taken.");
        }

        else if (command.Noun == "axe")
        {
            if (CurrentLocation.Name != "Amazon Rainforest Cave")
            {
                Console.WriteLine("You can't take the axe.");
            }
            else
            {
                Console.WriteLine("You steal the axe from the man.");
                Inventory.Add(item);
            }
        }
        
        else if (command.Noun == "woodenkey")
        {
            Console.WriteLine("You take the wooden key.");
            Inventory.Add(item);
        }
        
        else if (command.Noun == "diamondkey")
        {
            if (Conditions.IsTrue(ConditionTypes.IsStrengthened) == false)
            {
                Console.WriteLine("You can't take the diamond key, you are not strong enough.");
                RemoveItemFromInventory("diamondkey");
            }

            else if (Conditions.IsTrue(ConditionTypes.IsStrengthened))
            {
                Inventory.Add(item);
                Console.WriteLine("You take the diamondkey, it now feeling as light as any other key in your hand.");
            }
        }
        else
        {
            Inventory.Add(item);
            CurrentLocation.RemoveItem(item);
            item.Pickup();
        }
    }

    public static void ShowInventory()
    {
        if (Inventory.Count == 0)
        {
            Console.WriteLine("You are empty-handed.");
        }
        else
        {
            Console.WriteLine("You are carrying:");
            foreach (Item item in Inventory)
            {
                string article = SemanticTools.CreateArticle(item.Name);
                Console.WriteLine(" " + article + " " + item.Name);
            }
        }
    }

    public static void Look()
    {
        Console.WriteLine(CurrentLocation.GetDescription());
    }

    public static void Drop(Command command)
    {
        Item item = Items.GetItemByName(command.Noun);

        if (item == null)
        {
            string article = SemanticTools.CreateArticle(command.Noun);
            Console.WriteLine("I don't know what " + article + " " + command.Noun + " is.");
        }
        else if (!Inventory.Contains(item))
        {
            Console.WriteLine("You're not carrying the " + command.Noun + ".");
        }
        else
        {
            Inventory.Remove(item);
            CurrentLocation.AddItem(item);
            Console.WriteLine("You drop the " + command.Noun + ".");
        }

    }

    public static void Drink(Command command)
    {
        if (command.Noun == "beer")
        {
            Console.WriteLine("** drinking beer");
            Conditions.ChangeCondition(ConditionTypes.IsDrunk, true);
            RemoveItemFromInventory("beer");
            AddItemToInventory("beer-bottle");
        }

        if (command.Noun == "redpotion")
        {
            //also rewrite this bc they aren't takeable, can't have the item in inv, just have to be in the lab
            if (CurrentLocation.Name != "Laboratory")
            {
                Console.WriteLine("There are no potions to drink.");
            }
            else
            {
                Console.WriteLine("** drinking red potion");
                Conditions.ChangeCondition(ConditionTypes.IsStrengthened, true);
            }
        }

        if (command.Noun == "greenpotion")
        {
            //rewrite this to say if you're not in the lab there is no potion because it has to add it to your inv
            if (CurrentLocation.Name != "Laboratory")
            {
                Console.WriteLine("There are no potions to drink.");
            }
            else
            {
                Console.WriteLine("** drinking green potion");
                AddItemToInventory("greenpotion");
            }
        }
    }

    public static void AddItemToInventory(string itemName)
    {
        Item item = Items.GetItemByName(itemName);

        if (item == null)
        {
            return;
        }

        Inventory.Add(item);
    }

    public static void RemoveItemFromInventory(string itemName)
    {
        Item item = Items.GetItemByName(itemName);
        if (item == null)
        {
            return;
        }

        Inventory.Remove(item);
    }

    public static void MoveToLocation(string locationName)
    {
        // look up the location object based on the name
        Location newLocation = Map.GetLocationByName(locationName);

        // if there's no location with that name
        if (newLocation == null)
        {
            Console.WriteLine("Trying to move to unknown location: " + locationName + ".");
            return;
        }

        // set our current location to the new location
        CurrentLocation = newLocation;

        // print out a description of the location
        Look();
    }

    public static void Pull(Command command)
    {
        if (command.Noun == "lever")
        {
            Console.WriteLine("** pulling lever");
            Conditions.ChangeCondition(ConditionTypes.IsCreatedConnection, true);
        }
    }

    public static void Open(Command command)
    {
        if (command.Noun == "chest")
        {
            Console.WriteLine("you are too weak, come back later with a tool.");

        }
    }

    public static bool HasItem(string itemName)
    {
        return Inventory.Contains(Items.GetItemByName(itemName));
    }

    public static void Break(Command command)
    {
        if (command.Noun == "chest")
        {
            if (!HasItem("axe"))
            {
                Console.WriteLine("You don't have the right tool.");
            }
            else if (CurrentLocation.Name != "East Cave Wall")
            {
                Console.WriteLine("There's no chest here.");
            }
            else
            {
                // break the chest
                Console.WriteLine("You break the chest open with the frail axe.");
                Map.AddItem("woodenkey", "East Cave Wall");
                Map.RemoveItem("chest", "East Cave Wall");
                Console.WriteLine("There is an ancient key fashioned from moldy wood inside the broken chest.");
            }
        }
    }



    public static bool IsGameOver()
    {
        if (CurrentLocation.Name == "Storage")
        {
            Console.WriteLine("The door was booby trapped with a timed bomb, killing you instantly after 5 seconds.");
            return true;
        }

        if (CurrentLocation.Name == "Throne Room")
        {
            Console.WriteLine("The ghost of the fallen king haunts you to death, 'He strangles you'.");
            return true;
        }

        if (CurrentLocation.Name == "Pool")
        {
            Console.WriteLine(
                "You slip and hit your head because you were running on the wet tiles when you shouldn't have.");
            return true;
        }

        // if in lake
        if (CurrentLocation.Name == "Lake")
        {
            if (HasItem("raft"))
            {
                return false;
            }

            Console.WriteLine("The water comes alive and drowns you to death.");
            return true;
        }

        if (HasItem("greenpotion"))
        {
            Console.WriteLine(
                "You chose wrong. Instead of a delicious drink that empowers you, you have ingested a strong poison that burns you alive from the inside out.");
            return true;
        }

        // if has item in inventory


        return false;
    }

    public static void Climb(Command command)
    {
        if (CurrentLocation.Name is not ("East Cave" or "East Cave Wall"))
        {
            Console.WriteLine("There is no wall here. Try somewhere else.");
        }
        else if (command.Noun is "up" or "wall")
        {
            Console.WriteLine("** climbing up wall");
            MoveToLocation("East Cave Wall");
        }
        else if (command.Noun == "down")
        {
            Console.WriteLine("** climbing down wall");
            MoveToLocation("East Cave");
        }
    }

    public static void Swim(Command command)
    {
        if (CurrentLocation.Name != "Lake Cave")
        {
            Console.WriteLine("You can't swim here.");
        }
        else
        {
            MoveToLocation("Lake");
        }
    }

    public static void Cut(Command command)
    {
        if (command.Noun is ("tree" or "trees"))
        {
            if (!HasItem("axe"))
            {
                Console.WriteLine("You don't have the right tool.");
            }
            else if (CurrentLocation.Name != "Amazon Rainforest Cave")
            {
                Console.WriteLine("There are no trees here to cut.");
            }
            else
            {
                Console.WriteLine("You have cut the Brazil Nut tree.");
                Console.WriteLine("Now you have all this wood, probably enough to build a raft.");
                AddItemToInventory("wood");
            }
        }
    }

    public static void Build(Command command)
    {
        if (command.Noun == "raft")
        {
            if (!HasItem("wood"))
            {
                Console.WriteLine("You don't have enough wood.");
            }
            else
            {
                Console.WriteLine("You have built a raft out of wood.");
                RemoveItemFromInventory("wood");
                AddItemToInventory("raft");
            }
        }
    }
    public static void Help(Command command)
    {
        Console.WriteLine(
            "Command List: \n go \n take \n look \n inventory \n pull \n climb 'up/down' \n break \n cut \n build \n swim \n use \n drink");
    }

    public static void Search(Command command)
    {
        if (CurrentLocation.Name != "Bedroom")
        {
            Console.WriteLine("There is nothing to search here.");
        }
        
        else if (command.Noun == "closet")
        {
            Console.WriteLine("You look through all the dirty clothes and get some sticky stuff on your hands, " +
                              "but you can't seem to find anything useful here.");
        }

        else if (command.Noun == "desk")
        {
            Console.WriteLine("You search through the papers and trinkets strewn about the top of the desk, " +
                              "finding nothing. But then, you check the drawers and find a DIAMONDKEY sitting in one of them. " +
                              "It seems to be pretty heavy, though...");
            Map.AddItem("diamondkey", "Bedroom");
        }

        else if (command.Noun == "bed")
        {
            Console.WriteLine(
                "You pull the pile of sheets off the bed and look under the pillows, but there doesn't seem to be anything interesting here.");
        }
    }

    public static void Use(Command command)
        {
            if (command.Noun == "raft")
            {
                if (!HasItem("raft"))
                {
                    Console.WriteLine("You don't have a raft");
                }
                else
                {
                    Console.WriteLine("You sail across the lake safe and sound.");
                    MoveToLocation("Lake House");
                }
            }

            if (command.Noun == "woodenkey")
            {
                if (!HasItem("woodenkey"))
                {
                    Console.WriteLine("You don't have the key to unlock this door.");
                }
                else if (CurrentLocation.Name != "Lake House")
                {
                    Console.WriteLine("There is no lock here.");
                }
                else
                {
                    Console.WriteLine("** unlocking door");
                    Conditions.ChangeCondition(ConditionTypes.IsUnlockedHouse, true);
                    Console.WriteLine("You have unlocked the door. You can go south to enter the house.");
                }
            }

            if (command.Noun == "diamondkey")
                if (!HasItem("diamondkey"))
                {
                    Console.WriteLine("You don't have the key to unlock this door.");
                }
                else if (CurrentLocation.Name != "West Cave")
                {
                    Console.WriteLine("There is no lock here.");
                }
                else
                {
                    Console.WriteLine("** unlocking door");
                    Console.WriteLine("Congratulations!! You have unlocked the Diamond Door and secured your victory over the caves! \n The sunlight feels refreshing on your face, and you step out into the beautiful morning air. \n You can now safely type 'exit' to conclude the experience. Thanks for playing!");
                }
        }
    }
