using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContentLoader
{
    private const string COLOR_START_GRENDAR_INPUT = "<start_grendar_color>";
    private const string COLOR_STOP_GRENDAR_INPUT = "<stop_grendar_color>";

    private const string COLOR_START_GRENDAR_OUTPUT = "<color=#4FC878>";
    private const string COLOR_STOP_GRENDAR_OUTPUT = "</color>";

    private const string COLOR_START_TREE_INPUT = "<start_tree_color>";
    private const string COLOR_STOP_TREE_INPUT = "<stop_tree_color>";

    private const string COLOR_START_TREE_OUTPUT = "<color=#EF2D56>";
    private const string COLOR_STOP_TREE_OUTPUT = "</color>";

    private const string COLOR_START_SOUND_INPUT = "<start_sound_color>";
    private const string COLOR_STOP_SOUND_INPUT = "<stop_sound_color>";

    private const string COLOR_START_SOUND_OUTPUT = "<color=#ED7D3A>";
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

    private static Door ConvertDoorDataToDoor(JSONNode node)
    {
        Door door = new Door();
        door.id = node["id"];
        door.name = node["name"].Value.ToUpper();
        door.shortDescription = node["shortDescription"];
        door.extendedDescription = node["extendedDescription"];

        door.extendedDescription = door.extendedDescription.Replace(COLOR_START_GRENDAR_INPUT, COLOR_START_GRENDAR_OUTPUT);
        door.extendedDescription = door.extendedDescription.Replace(COLOR_STOP_GRENDAR_INPUT, COLOR_STOP_GRENDAR_OUTPUT);

        door.extendedDescription = door.extendedDescription.Replace(COLOR_START_TREE_INPUT, COLOR_START_TREE_OUTPUT);
        door.extendedDescription = door.extendedDescription.Replace(COLOR_STOP_TREE_INPUT, COLOR_STOP_TREE_OUTPUT);

        door.extendedDescription = door.extendedDescription.Replace(COLOR_START_SOUND_INPUT, COLOR_START_SOUND_OUTPUT);
        door.extendedDescription = door.extendedDescription.Replace(COLOR_STOP_SOUND_INPUT, COLOR_STOP_SOUND_OUTPUT);

        door.extendedDescriptionBlock = new List<string>();
        foreach (var descriptionEntry in node["extendedDescriptionBlock"] as JSONArray)
        {
            string value = descriptionEntry.Value as JSONString;
            door.extendedDescriptionBlock.Add(value);
        }

        List<string> roomIds = new List<string>();
        foreach (var roomId in node["roomIds"] as JSONArray)
        {
            roomIds.Add(roomId.Value as JSONString);
        }

        int flagsCounter = 0;
        foreach (var verbIndex in node["verbs"] as JSONArray)
        {
            int value = (verbIndex.Value as JSONNumber).AsInt;
            flagsCounter += value;
        }

        door.validVerbs = (Verb)flagsCounter;
        
        door.roomIds = roomIds;
        door.locked = node["locked"];
        door.closed = node["closed"];

        return door;
    }

    private static Room ConvertRoomDataToRoom(JSONNode node)
    {
        Room room = new Room();
        room.id = node["id"];
        room.name = node["name"].Value.ToUpper();
        room.shortDescription = node["shortDescription"];
        room.extendedDescription = node["extendedDescription"];
        room.selfDescription = node["selfDescription"];

        room.selfDescription = room.selfDescription.Replace(COLOR_START_GRENDAR_INPUT, COLOR_START_GRENDAR_OUTPUT);
        room.selfDescription = room.selfDescription.Replace(COLOR_STOP_GRENDAR_INPUT, COLOR_STOP_GRENDAR_OUTPUT);

        room.selfDescription = room.selfDescription.Replace(COLOR_START_TREE_INPUT, COLOR_START_TREE_OUTPUT);
        room.selfDescription = room.selfDescription.Replace(COLOR_STOP_TREE_INPUT, COLOR_STOP_TREE_OUTPUT);

        room.selfDescription = room.selfDescription.Replace(COLOR_START_SOUND_INPUT, COLOR_START_SOUND_OUTPUT);
        room.selfDescription = room.selfDescription.Replace(COLOR_STOP_SOUND_INPUT, COLOR_STOP_SOUND_OUTPUT);

        room.selfDescriptionBlock = new List<string>();

        foreach (var selfDescriptionEntry in node["selfDescriptionBlock"] as JSONArray)
        {
            string value = selfDescriptionEntry.Value as JSONString;
            value = value.Replace(COLOR_START_GRENDAR_INPUT, COLOR_START_GRENDAR_OUTPUT);
            value = value.Replace(COLOR_STOP_GRENDAR_INPUT, COLOR_STOP_GRENDAR_OUTPUT);

            value = value.Replace(COLOR_START_TREE_INPUT, COLOR_START_TREE_OUTPUT);
            value = value.Replace(COLOR_STOP_TREE_INPUT, COLOR_STOP_TREE_OUTPUT);

            value = value.Replace(COLOR_START_SOUND_INPUT, COLOR_START_SOUND_OUTPUT);
            value = value.Replace(COLOR_STOP_SOUND_INPUT, COLOR_STOP_SOUND_OUTPUT);
            room.selfDescriptionBlock.Add(value);
        }

        room.extendedDescriptionBlock = new List<string>();
        foreach (var descriptionEntry in node["extendedDescriptionBlock"] as JSONArray)
        {
            string value = descriptionEntry.Value as JSONString;
            room.extendedDescriptionBlock.Add(value);
        }

        room.imageSpriteIndex = node["imageSpriteIndex"] as JSONNumber;

        int flagsCounter = 0;
        foreach (var verbIndex in node["verbs"] as JSONArray)
        {
            int value = (verbIndex.Value as JSONNumber).AsInt;
            flagsCounter += value;
        }

        room.validVerbs = (Verb)flagsCounter;

        List<string> doorIds = new List<string>();
        foreach (var roomId in node["doorIds"] as JSONArray)
        {
            doorIds.Add(roomId.Value as JSONString);
        }

        room.doorIds = doorIds;

        List<string> itemIds = new List<string>();
        foreach (var roomId in node["itemIds"] as JSONArray)
        {
            itemIds.Add(roomId.Value as JSONString);
        }

        room.itemIds = itemIds;

        return room;
    }

    public static Item ConvertItemDataToItem(JSONNode node)
    {
        Item item = new Item();
        item.id = node["id"];
        item.name = node["name"].Value.ToUpper();
        item.shortDescription = node["shortDescription"];
        item.extendedDescription = node["extendedDescription"];

        item.extendedDescriptionBlock = new List<string>();
        foreach (var descriptionEntry in node["extendedDescriptionBlock"] as JSONArray)
        {
            string value = descriptionEntry.Value as JSONString;
            item.extendedDescriptionBlock.Add(value);
        }

        item.useItemTextBlock = new List<string>();
        foreach (var entry in node["useItemTextBlock"] as JSONArray)
        {
            string value = entry.Value as JSONString;
            item.useItemTextBlock.Add(value);
        }

        int flagsCounter = 0;
        foreach (var verbIndex in node["verbs"] as JSONArray)
        {
            int value = (verbIndex.Value as JSONNumber).AsInt;
            flagsCounter += value;
        }

        item.validVerbs = (Verb)flagsCounter;

        List<string> doorIdsToUnlock = new List<string>();
        foreach (var doorId in node["doorIdsToUnlock"] as JSONArray)
        {
            doorIdsToUnlock.Add(doorId.Value as JSONString);
        }

        item.doorIdsToUnlock = doorIdsToUnlock;

        List<string> itemIdsToUnlock = new List<string>();
        foreach (var itemId in node["itemIdsToUnlock"] as JSONArray)
        {
            itemIdsToUnlock.Add(itemId.Value as JSONString);
        }

        item.itemIdsToUnlock = itemIdsToUnlock;

        item.useItemText = node["useItemText"] as JSONString;

        return item;
    }
}
