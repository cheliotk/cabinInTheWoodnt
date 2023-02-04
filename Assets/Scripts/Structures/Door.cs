using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataStructures;

[Serializable]
public class Door : ObjectBase
{
    public List<string> roomIds;
    public bool closed;
    public bool locked;

    public GoThroughDoorResult GoThroughDoor(Room roomFrom, List<Room> availableRooms)
    {
        if (locked)
            return new GoThroughDoorResult(false, roomFrom, null, true, true);
        if(closed)
            return new GoThroughDoorResult(false, roomFrom, null, false, true);

        foreach (var roomId in roomIds)
        {
            if (roomFrom.id == roomId)
                continue;

            Room newRoom = availableRooms.Find(x => x.id == roomId);
            return new GoThroughDoorResult(true, roomFrom, newRoom, true, true);
        }

        return new GoThroughDoorResult(false, roomFrom, null, false, false);
    }

    public OpenDoorResult OpenDoor()
    {
        if (locked)
            return new OpenDoorResult(false, true, false);
        if (closed)
        {
            closed = false;
            return new OpenDoorResult(true, false, false);
        }
        if (!closed)
            return new OpenDoorResult(false, false, true);

        return new OpenDoorResult(false, false, false);
    }
}
