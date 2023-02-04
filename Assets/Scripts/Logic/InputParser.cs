using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if(words.Length > 2 )
        {
            onInputParsed?.Invoke(instance, new ParserResultArgs(false, null, null, ParserFailType.INPUT_TOO_LONG));
        }
    }

    public static VerbCheckResult HasVerb(string input)
    {
        foreach (string verb in Vocabulary.Verbs)
        {
            //foreach (string verbSynonym in Vocabulary.VerbSynonyms[verb])
            //{
                if(input.Contains(verb))
                {
                    return new VerbCheckResult(true, verb);
                }
            //}
        }

        return new VerbCheckResult(false, null);
    }

    public static TargetCheckResult HasTarget(string input, List<Door> doors, List<Item> items)
    {
        foreach(Door door in doors)
        {
            if(input.Contains(door.name))
            {
                return new TargetCheckResult(true, TargetCheckType.DOOR, door);
            }
        }

        foreach (Item item in items)
        {
            if(input.Contains(item.name))
            {
                return new TargetCheckResult(true, TargetCheckType.ITEM, item);
            }
        }

        return new TargetCheckResult(false, TargetCheckType.NONE, null);
    }
}
