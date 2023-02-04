using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static DataStructures;

public class GameManager : MonoBehaviour
{
    public List<Room> rooms;
    public List<Door> doors;
    public string currentRoomId;

    private InputParser inputParser;
    private Vocabulary vocabulary;
    [SerializeField] private TextController textController;
    [SerializeField] private TMP_InputField inputField;

    private void Start()
    {
        inputParser = new InputParser();
        vocabulary = new Vocabulary(new List<string>() { "OPEN", "LOOK", "GO THROUGH" }, new Dictionary<string, List<string>>());
        SetupRooms();
    }

    private void SetupRooms()
    {
        foreach (Room room in rooms)
        {
            room.SetupDoors(doors);
        }
    }

    public void GoToRoom(Room room)
    {
        currentRoomId = room.id;
    }

    public void ParseInput(string input)
    {
        VerbCheckResult verbResult = InputParser.HasVerb(input);

        Room currentRoom = rooms.Find(x => x.id == currentRoomId);
        List<Door> doorsInRoom = doors.FindAll(x => currentRoom.doors.Contains(x));
        List<Item> itemsInRoom = currentRoom.items;

        TargetCheckResult targetResult = InputParser.HasTarget(input, doorsInRoom, itemsInRoom);
        if (targetResult.success && targetResult.target == null)
        {
            targetResult.target = currentRoom;
        }

        if (verbResult.success && targetResult.success)
        {
            bool canCombine = targetResult.target.validVerbs.HasFlag(verbResult.verb);
            string successText = canCombine ? "LETS TRY" : "NAH";
            ShowAcknowledgementText($"You want to {verbResult.verbString} {targetResult.target.name}? {successText}");

            if (canCombine)
            {
                if (targetResult.target is Door targetDoor)
                {
                    InteractWithDoor(targetDoor, verbResult, currentRoom);
                }
                else if (targetResult.target is Item targetItem)
                {

                }
                else if (targetResult.target is Room room)
                {
                    InteractWithRoom(room, verbResult);
                }
            }
        }
        else if (verbResult.success)
        {
            if (verbResult.verb == Verb.LOOK)
            {
                ShowAcknowledgementText($"You want to {verbResult.verbString}? SURE");
                InteractWithRoom(currentRoom, verbResult);
            }
        }
        else if (targetResult.success)
        {
            ShowAcknowledgementText($"You want to look at {targetResult.target.name}? SURE");
            ShowText($"{targetResult.target.description}");
        }
        else
        {
            ShowAcknowledgementText($"I don't understand what you want me to do");
        }
    }

    private void InteractWithRoom(Room currentRoom, VerbCheckResult verbResult)
    {
        if (verbResult.verb == Verb.LOOK)
        {
            string roomDescription = currentRoom.description;

            string doorsDesc = currentRoom.GetDoorsString();
            if (doorsDesc != string.Empty)
                roomDescription += doorsDesc;
            
            string itemsDesc = currentRoom.GetItemsString();
            if (itemsDesc != string.Empty)
                roomDescription += itemsDesc;

            ShowText($"{roomDescription}");
        }
    }

    private void InteractWithDoor(Door targetDoor, VerbCheckResult verbResult, Room currentRoom)
    {
        if (verbResult.verb == Verb.GO_THROUGH)
        {
            GoThroughDoorResult passDoorResult = targetDoor.GoThroughDoor(currentRoom, rooms);
            if (passDoorResult.success)
            {
                currentRoomId = passDoorResult.newRoom.id;
                ShowText($"You passed through the door");
            }
            else
            {
                if (passDoorResult.locked)
                {
                    ShowText($"The door is locked");
                }
                else if (passDoorResult.closed)
                {
                    ShowText($"The door is closed");
                }
            }
        }
        else if (verbResult.verb == Verb.LOOK)
        {
            ShowText($"{targetDoor.description}");
        }
        else if (verbResult.verb == Verb.OPEN)
        {
            var result = targetDoor.OpenDoor();
            if (result.success)
            {
                ShowText($"The door is now open.");
            }
            else
            {
                if (result.locked)
                {
                    ShowText($"The door is locked.");
                }
                else if (result.alreadyOpen)
                {
                    ShowText($"The door is already open.");
                }
            }
        }
    }

    private void ShowText(string text)
    {
        textController.AddText(text);
        inputField.text = "";
    }

    private void ShowAcknowledgementText(string text)
    {
        textController.AddAcknowledgementText(text);
        inputField.text = "";
    }
}
