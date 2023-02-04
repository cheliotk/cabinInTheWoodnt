using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContentLoader
{
    public static void LoadGameDoors(TextAsset doorsFile)
    {
        JSONNode doorsRaw = JSON.Parse(doorsFile.text);
        foreach (KeyValuePair<string, JSONNode> entry in doorsRaw as JSONArray)
        {
            Door door = ConvertDoorDataToDoor(entry.Value);
            Debug.Log(door);
        }
    }

    private static Door ConvertDoorDataToDoor(JSONNode doorNode)
    {
        Door door = new Door();
        door.id = doorNode["id"];
        door.name = doorNode["name"];
        door.description = doorNode["description"];
        
        List<string> roomIds = new List<string>();
        foreach (var roomId in doorNode["roomIds"] as JSONArray)
        {
            roomIds.Add(roomId.Value as JSONString);
        }

        int flagsCounter = 0;
        foreach (var verbIndex in doorNode["verbs"] as JSONArray)
        {
            int value = (verbIndex.Value as JSONNumber).AsInt;
            flagsCounter += value;
        }

        door.validVerbs = (Verb)flagsCounter;
        
        door.roomIds = roomIds;
        door.locked = doorNode["locked"];
        door.closed = doorNode["closed"];

        return door;
    }
}
