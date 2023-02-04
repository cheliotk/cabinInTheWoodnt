using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Door : ObjectBase
{
    public List<int> roomIds;
    public bool locked;
}
