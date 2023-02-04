using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataStructures;

public class GameManager : MonoBehaviour
{
    public List<Room> rooms;
    public int currentRoomId;

    private InputParser inputParser;
    private Vocabulary vocabulary;

    private void Start()
    {
        inputParser = new InputParser();
        vocabulary = new Vocabulary(new List<string>() { "OPEN", "LOOK" }, new Dictionary<string, List<string>>());
    }

    public void GoToRoom(Room room)
    {

    }

    public void ParseInput(string input)
    {
        VerbCheckResult verbResult = InputParser.HasVerb(input);
        
        Room currentRoom = rooms[currentRoomId];
        List<Door> doorsInRoom = currentRoom.connections;
        List<Item> itemsInRoom = currentRoom.items;

        TargetCheckResult targetResult = InputParser.HasTarget(input, doorsInRoom, itemsInRoom);

        if(verbResult.success && targetResult.success)
        {
            string target = "";
            switch (targetResult.targetType)
            {
                case TargetCheckType.NONE:
                    break;
                case TargetCheckType.ITEM:
                    target = targetResult.targetItem.name;
                    break;
                case TargetCheckType.DOOR:
                    target = targetResult.targetDoor.name;
                    break;
                default:
                    break;
            }
            Debug.Log($"You want to {verbResult.verb} {target}");
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
