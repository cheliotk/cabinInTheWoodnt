using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataStructures;

public class GameManager : MonoBehaviour
{
    public List<Room> rooms;
    public List<Door> doors;
    public string currentRoomId;

    private InputParser inputParser;
    private Vocabulary vocabulary;

    private void Start()
    {
        inputParser = new InputParser();
        vocabulary = new Vocabulary(new List<string>() { "OPEN", "LOOK", "GO THROUGH" }, new Dictionary<string, List<string>>());
    }

    public void GoToRoom(Room room)
    {
        currentRoomId = room.id;
    }

    public void ParseInput(string input)
    {
        VerbCheckResult verbResult = InputParser.HasVerb(input);
        
        Room currentRoom = rooms.Find(x => x.id == currentRoomId);
        List<Door> doorsInRoom = doors.FindAll(x => currentRoom.doorIds.Contains(x.id));
        List<Item> itemsInRoom = currentRoom.items;

        TargetCheckResult targetResult = InputParser.HasTarget(input, doorsInRoom, itemsInRoom);

        if(verbResult.success && targetResult.success)
        {
            bool canCombine = targetResult.target.validVerbs.Contains(verbResult.verb);
            string successText = canCombine ? "SURE" : "NAH";
            Debug.Log($"You want to {verbResult.verb} {targetResult.target.name}? {successText}");
        }
        else if (verbResult.success)
        {
            if(verbResult.verb == "LOOK")
            {
                Debug.Log($"You want to {verbResult.verb}");
            }
        }
        else
        {
            Debug.Log($"I don't understand what you want to do");
        }
    }
}
