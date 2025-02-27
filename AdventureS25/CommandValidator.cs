﻿namespace AdventureS25;

public static class CommandValidator
{
    private static float MaxSpeed;
    
    public static List<string> Verbs = new List<string>
        {"go", "eat"};
    
    public static List<string> StandaloneVerbs = new List<string>
        {"exit", "inventory", "look"};
    
    public static List<string> Nouns = new List<string>
        {"bagel", "apple", "beer"};
    
    public static bool IsValid(Command command)
    {
        bool isValid = false;
        
        if (IsVerb(command.Verb) || IsStandaloneVerb(command.Verb))
        {
            if (IsStandaloneVerb(command.Verb))
            {
                isValid = true;
            }
            else if (IsNoun(command.Noun))
            {
                isValid = true;
            }
        }
            
        return isValid;
    }
    
    private static bool IsNoun(string commandNoun)
    {
        if (Nouns.Contains(commandNoun))
        {
            return true;
        }
        return false;
    }

    private static bool IsStandaloneVerb(string commandVerb)
    {
        if (StandaloneVerbs.Contains(commandVerb))
        {
            return true;
        }
        return false;
    }

    private static bool IsVerb(string commandVerb)
    {
        if (Verbs.Contains(commandVerb))
        {
            return true;
        }
        
        return false;
    }   
}