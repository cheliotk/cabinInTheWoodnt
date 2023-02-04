using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Room : ObjectBase
{
    public List<string> doorIds;

    public List<Item> items;
}
