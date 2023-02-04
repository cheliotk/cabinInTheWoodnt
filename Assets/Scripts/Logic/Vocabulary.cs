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
            { "GO THROUGH", Verb.GO_THROUGH },
            { "USE", Verb.USE }
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
    GO_THROUGH = 4,
    USE = 8
}
