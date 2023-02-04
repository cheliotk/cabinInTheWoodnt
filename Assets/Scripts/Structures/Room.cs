using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Room : ObjectBase
{
    public List<string> doorIds;
    public List<Door> doors { get; private set; }

    public List<Item> items;

    public void SetupDoors(List<Door> allDoors)
    {
        doors = new List<Door>();
        foreach (string id in doorIds)
        {
            doors.Add(allDoors.Find(x=> x.id == id));
        }
    }

    public string GetDoorsString()
    {
        string doorsString = string.Empty;
        foreach (Door door in doors)
        {
            doorsString += "\n" + door.description;
        }

        return doorsString;
    }

    public string GetItemsString()
    {
        string itemsString = string.Empty;
        foreach (Item item in items)
        {
            itemsString += "\n" + item.description;
        }
        return itemsString;
    }
}
