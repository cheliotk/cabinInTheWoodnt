using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Vocabulary
{
    public static List<string> Verbs;
    public static Dictionary<string, List<string>> VerbSynonyms;
    private static Vocabulary instance;

    public Vocabulary(List<string> verbs, Dictionary<string, List<string>> verbSynonyms)
    {
        instance = this;
        Verbs = verbs;
        VerbSynonyms = verbSynonyms;
    }
}
