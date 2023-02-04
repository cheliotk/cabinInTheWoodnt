using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Door : ObjectBase
{
    public List<string> roomIds;
    public bool locked;

    public Room GoThroughDoor(Room roomFrom, List<Room> availableRooms)
    {
        foreach(var roomId in roomIds)
        {
            if (roomFrom.id == roomId)
                continue;

            return availableRooms.Find(x => x.id == roomId);
        }

        return null;
    }
}
