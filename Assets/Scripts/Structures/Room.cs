using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Room : ObjectBase
{
    public string selfDescription;
    public List<string> doorIds;
    public List<Door> doors { get; private set; }

    public List<string> itemIds;
    public List<Item> items { get; private set; }
    public int imageSpriteIndex;

    public void SetupDoors(List<Door> allDoors)
    {
        doors = new List<Door>();
        foreach (string id in doorIds)
        {
            doors.Add(allDoors.Find(x=> x.id == id));
        }
    }

    public void SetupItems(List<Item> allItems)
    {
        items = new List<Item>();
        foreach (string id in itemIds)
        {
            items.Add(allItems.Find(x => x.id == id));
        }
    }

    public void AddItem(Item item)
    {
        itemIds.Add(item.id);
        items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        itemIds.Remove(item.id);
        items.Remove(item);
    }

    public string GetDoorsString()
    {
        string doorsString = string.Empty;
        foreach (Door door in doors)
        {
            doorsString += "\n" + door.shortDescription;
        }

        return doorsString;
    }

    public string GetItemsString()
    {
        string itemsString = string.Empty;
        foreach (Item item in items)
        {
            itemsString += "\n" + item.shortDescription;
        }
        return itemsString;
    }
}
