﻿
using System.Net.Sockets;

namespace AdventureS25;

public static class Conditions
{
    public static Dictionary<ConditionTypes, Condition> conditions 
        = new Dictionary<ConditionTypes, Condition>();
    
    public static void Initialize()
    {
        Condition isDrunk = new Condition(ConditionTypes.IsDrunk);
        isDrunk.AddToActivateList(ConditionActions.WriteOutput("Hic!"));
        Add(isDrunk);

        Condition beerMe = new Condition(ConditionTypes.IsBeerMed);
        beerMe.AddToActivateList(ConditionActions.AddItemToInventory("beer"));
        beerMe.AddToDeactivateList(ConditionActions.RemoveItemFromInventory("beer"));
        Add(beerMe);

        Condition isHungover = new Condition(ConditionTypes.IsHungover);
        isHungover.AddToActivateList(ConditionActions.AddItemToLocation("puke", 
            "Entrance"));
        Add(isHungover);

        Condition isTidyedUp = new Condition(ConditionTypes.IsTidiedUp);
        isTidyedUp.AddToActivateList(ConditionActions.RemoveItemFromLocation("puke",
            "Entrance"));
        Add(isTidyedUp);
        
        Condition isTeleported = new Condition(ConditionTypes.IsTeleported);
        isTeleported.AddToActivateList(ConditionActions.MovePlayerToLocation("Throne Room"));
        Add(isTeleported);

        Condition isConnectedRooms = new Condition(ConditionTypes.IsCreatedConnection);
        isConnectedRooms.AddToActivateList(ConditionActions.WriteOutput("An opening into a cave has magically appeared to the south."));
        isConnectedRooms.AddToActivateList(ConditionActions.AddMapConnection("Entrance", 
            "south", "Cave"));
        //isConnectedRooms.AddToActivateList(ConditionActions.AddMapConnection("Cave", 
           // "north", "Entrance"));
        Add(isConnectedRooms);

        Condition isDisconnectedRooms = new Condition(ConditionTypes.IsRemovedConnection);
        isDisconnectedRooms.AddToActivateList(ConditionActions.WriteOutput("A convenient pile of rocks blocks the passage between the entrance and the throne room."));
        isDisconnectedRooms.AddToActivateList(ConditionActions.RemoveMapConnection("Entrance", "north"));
        isDisconnectedRooms.AddToActivateList(ConditionActions.RemoveMapConnection("Throne Room", "south"));
        Add(isDisconnectedRooms);
        
        Condition IsDead = new Condition(ConditionTypes.IsDead);
        IsDead.AddToActivateList(ConditionActions.EndGame());
        Add(IsDead);

        Condition isConnectedHouse = new Condition(ConditionTypes.IsUnlockedHouse);
        isConnectedHouse.AddToActivateList(ConditionActions.AddMapConnection("Lake House", "south", "Living Room"));
        isConnectedHouse.AddToActivateList(ConditionActions.AddMapConnection("Living Room", "north", "Lake House"));
        Add(isConnectedHouse);
        
        Condition isStrengthened = new Condition(ConditionTypes.IsStrengthened);
        isStrengthened.AddToActivateList(ConditionActions.WriteOutput("You drink the redpotion and feel a new vigor course through your veins. You feel as though you could lift anything."));
        Add(isStrengthened);
    }   
    
    public static void ChangeCondition(ConditionTypes conditionType,
        bool isSettingToTrue)
    {
        if (NotInDictionary(conditionType))
        {
            return;
        }
        
        // if setting to true AND we're currently false - we can set to true
        if (isSettingToTrue && IsFalse(conditionType))
        {
            Condition condition = conditions[conditionType];
            condition.Activate();
        }
        // if not setting to true and we're current true - we can set to false
        else if (!isSettingToTrue && IsTrue(conditionType))
        {
            Condition condition = conditions[conditionType];
            condition.Deactivate();
        }

    }

    public static void Add(Condition condition)
    {
        conditions.Add(condition.Type, condition);
    }

    public static bool IsTrue(ConditionTypes conditionType) 
    {
        if (NotInDictionary(conditionType))
        {
            return false;
        }

        return conditions[conditionType].IsActive;
    }

    public static bool IsFalse(ConditionTypes conditionType)
    {
        if (NotInDictionary(conditionType))
        {
            return true;
        }
        return !conditions[conditionType].IsActive;
    }

    private static bool NotInDictionary(ConditionTypes conditionType)
    {
        return !conditions.ContainsKey(conditionType);
    }
    
}
