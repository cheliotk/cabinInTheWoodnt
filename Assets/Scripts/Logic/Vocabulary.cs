using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Vocabulary
{
    public static List<string> verbs;
    public static Dictionary<string, List<string>> verbSynonyms;
    private static Dictionary<string, Verb> stringToVerbsDict;

    private static Vocabulary instance;

    public Vocabulary(List<string> verbs, Dictionary<string, List<string>> verbSynonyms)
    {
        instance = this;
        Vocabulary.verbs = verbs;
        Vocabulary.verbSynonyms = verbSynonyms;

        stringToVerbsDict = new Dictionary<string, Verb>
        {
            { "LOOK", Verb.LOOK },
            { "OPEN", Verb.OPEN },
            { "ENTER", Verb.ENTER },
            { "USE", Verb.USE },
            { "GO TO", Verb.GO_TO }
        };
    }

    public static Verb VerbStrToEnum(string str)
    {
        if (stringToVerbsDict.ContainsKey(str))
            return stringToVerbsDict[str];
        else
            return Verb.NONE;
    }
}

[Flags]
public enum Verb
{
    NONE = 0,
    LOOK = 1,
    OPEN = 2,
    ENTER = 4,
    USE = 8,
    GO_TO = 16
}
