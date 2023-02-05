using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContentLoader
{
    private const string COLOR_START_GRENDAR_INPUT = "<start_grendar_color>";
    private const string COLOR_STOP_GRENDAR_INPUT = "<stop_grendar_color>";

    private const string COLOR_START_GRENDAR_OUTPUT = "<color=#0CA789>";
    private const string COLOR_STOP_GRENDAR_OUTPUT = "</color>";

    private const string COLOR_START_TREE_INPUT = "<start_tree_color>";
    private const string COLOR_STOP_TREE_INPUT = "<stop_tree_color>";

    private const string COLOR_START_TREE_OUTPUT = "<color=#DA0063>";
    private const string COLOR_STOP_TREE_OUTPUT = "</color>";

    private const string COLOR_START_SOUND_INPUT = "<start_sound_color>";
    private const string COLOR_STOP_SOUND_INPUT = "<stop_sound_color>";

    private const string COLOR_START_SOUND_OUTPUT = "<color=#FFFFFF>";
    private const string COLOR_STOP_SOUND_OUTPUT = "</color>";


    public static List<Door> LoadGameDoors(TextAsset doorsFile)
    {
        List<Door> doors = new List<Door>();
        JSONNode doorsRaw = JSON.Parse(doorsFile.text);
        foreach (KeyValuePair<string, JSONNode> entry in doorsRaw as JSONArray)
        {
            Door door = ConvertDoorDataToDoor(entry.Value);
            doors.Add(door);
        }

        return doors;
    }

    public static List<Room> LoadGameRooms(TextAsset roomsFile)
    {
        List<Room> rooms = new List<Room>();
        JSONNode roomsRaw = JSON.Parse(roomsFile.text);
        foreach (KeyValuePair<string, JSONNode> entry in roomsRaw)
        {
            Room room = ConvertRoomDataToRoom(entry.Value);
            rooms.Add(room);
        }

        return rooms;
    }

    public static List<Item> LoadGameItems(TextAsset itemsFile)
    {
        List<Item> items = new List<Item>();
        JSONNode itemsRaw = JSON.Parse(itemsFile.text);
        foreach (KeyValuePair<string, JSONNode> entry in itemsRaw)
        {
            Item item = ConvertItemDataToItem(entry.Value);
            items.Add(item);
        }

        return items;
    }

    private static Door ConvertDoorDataToDoor(JSONNode doorNode)
    {
        Door door = new Door();
        door.id = doorNode["id"];
        door.name = doorNode["name"].Value.ToUpper();
        door.shortDescription = doorNode["shortDescription"];
        door.extendedDescription = doorNode["extendedDescription"];

        door.extendedDescription = door.extendedDescription.Replace(COLOR_START_GRENDAR_INPUT, COLOR_START_GRENDAR_OUTPUT);
        door.extendedDescription = door.extendedDescription.Replace(COLOR_STOP_GRENDAR_INPUT, COLOR_STOP_GRENDAR_OUTPUT);

        door.extendedDescription = door.extendedDescription.Replace(COLOR_START_TREE_INPUT, COLOR_START_TREE_OUTPUT);
        door.extendedDescription = door.extendedDescription.Replace(COLOR_STOP_TREE_INPUT, COLOR_STOP_TREE_OUTPUT);

        door.extendedDescription = door.extendedDescription.Replace(COLOR_START_SOUND_INPUT, COLOR_START_SOUND_OUTPUT);
        door.extendedDescription = door.extendedDescription.Replace(COLOR_STOP_SOUND_INPUT, COLOR_STOP_SOUND_OUTPUT);

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

    private static Room ConvertRoomDataToRoom(JSONNode roomNode)
    {
        Room room = new Room();
        room.id = roomNode["id"];
        room.name = roomNode["name"].Value.ToUpper();
        room.shortDescription = roomNode["shortDescription"];
        room.extendedDescription = roomNode["extendedDescription"];
        room.selfDescription = roomNode["selfDescription"];

        room.selfDescription = room.selfDescription.Replace(COLOR_START_GRENDAR_INPUT, COLOR_START_GRENDAR_OUTPUT);
        room.selfDescription = room.selfDescription.Replace(COLOR_STOP_GRENDAR_INPUT, COLOR_STOP_GRENDAR_OUTPUT);

        room.selfDescription = room.selfDescription.Replace(COLOR_START_TREE_INPUT, COLOR_START_TREE_OUTPUT);
        room.selfDescription = room.selfDescription.Replace(COLOR_STOP_TREE_INPUT, COLOR_STOP_TREE_OUTPUT);

        room.selfDescription = room.selfDescription.Replace(COLOR_START_SOUND_INPUT, COLOR_START_SOUND_OUTPUT);
        room.selfDescription = room.selfDescription.Replace(COLOR_STOP_SOUND_INPUT, COLOR_STOP_SOUND_OUTPUT);

        room.imageSpriteIndex = roomNode["imageSpriteIndex"] as JSONNumber;

        int flagsCounter = 0;
        foreach (var verbIndex in roomNode["verbs"] as JSONArray)
        {
            int value = (verbIndex.Value as JSONNumber).AsInt;
            flagsCounter += value;
        }

        room.validVerbs = (Verb)flagsCounter;

        List<string> doorIds = new List<string>();
        foreach (var roomId in roomNode["doorIds"] as JSONArray)
        {
            doorIds.Add(roomId.Value as JSONString);
        }

        room.doorIds = doorIds;

        List<string> itemIds = new List<string>();
        foreach (var roomId in roomNode["itemIds"] as JSONArray)
        {
            itemIds.Add(roomId.Value as JSONString);
        }

        room.itemIds = itemIds;

        return room;
    }

    public static Item ConvertItemDataToItem(JSONNode itemNode)
    {
        Item item = new Item();
        item.id = itemNode["id"];
        item.name = itemNode["name"].Value.ToUpper();
        item.shortDescription = itemNode["shortDescription"];
        item.extendedDescription = itemNode["extendedDescription"];

        int flagsCounter = 0;
        foreach (var verbIndex in itemNode["verbs"] as JSONArray)
        {
            int value = (verbIndex.Value as JSONNumber).AsInt;
            flagsCounter += value;
        }

        item.validVerbs = (Verb)flagsCounter;

        List<string> doorIdsToUnlock = new List<string>();
        foreach (var doorId in itemNode["doorIdsToUnlock"] as JSONArray)
        {
            doorIdsToUnlock.Add(doorId.Value as JSONString);
        }

        item.doorIdsToUnlock = doorIdsToUnlock;

        List<string> itemIdsToUnlock = new List<string>();
        foreach (var itemId in itemNode["itemIdsToUnlock"] as JSONArray)
        {
            itemIdsToUnlock.Add(itemId.Value as JSONString);
        }

        item.itemIdsToUnlock = itemIdsToUnlock;

        item.useItemText = itemNode["useItemText"] as JSONString;

        return item;
    }
}
