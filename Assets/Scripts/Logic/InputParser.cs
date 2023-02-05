using System;
using System.Collections.Generic;
using static DataStructures;

public class InputParser
{
    private static InputParser instance;

    public InputParser()
    {
        instance = this;
    }

    static event EventHandler<ParserResultArgs> onInputParsed;
    public static void ParseInput(string input)
    {
        string[] words = input.Split(' ');
        if (words.Length > 2)
        {
            onInputParsed?.Invoke(instance, new ParserResultArgs(false, null, null, ParserFailType.INPUT_TOO_LONG));
        }
    }

    public static VerbCheckResult HasVerb(string input)
    {
        input = input.ToUpper();
        foreach (string verb in Vocabulary.verbs)
        {
            //foreach (string verbSynonym in Vocabulary.VerbSynonyms[verb])
            //{
            if (input.Contains(verb))
            {
                return new VerbCheckResult(true, verb, Vocabulary.VerbStrToEnum(verb));
            }
            //}
        }

        return new VerbCheckResult(false, null, Verb.NONE);
    }

    public static TargetCheckResult HasTarget(string input, Room currentRoom)
    {
        input = input.ToUpper();

        if (input.Contains(currentRoom.name))
        {
            return new TargetCheckResult(true, TargetCheckType.ROOM, currentRoom);
        }

        List<Door> doors = currentRoom.doors;
        List<Item> items = currentRoom.items;

        foreach (Door door in doors)
        {
            if (input.Contains(door.name))
            {
                return new TargetCheckResult(true, TargetCheckType.DOOR, door);
            }
        }

        foreach (Item item in items)
        {
            if (input.Contains(item.name))
            {
                return new TargetCheckResult(true, TargetCheckType.ITEM, item);
            }
        }

        if (input.Contains("ROOM"))
        {
            return new TargetCheckResult(true, TargetCheckType.ROOM, null);
        }

        return new TargetCheckResult(false, TargetCheckType.NONE, null);
    }
}
