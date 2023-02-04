using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item : ObjectBase
{
    public List<string> itemIdsToUnlock;
    public List<string> doorIdsToUnlock;
    public string useItemText;
}
