using System;
using System.Collections.Generic;
using UnityEngine;
using static Vocabulary;

[Serializable]
public class ObjectBase
{
    public string id;
    public string name;
    public string description;
    public Verb validVerbs;
}
