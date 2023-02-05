using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static DataStructures;

public class GameManager : MonoBehaviour
{
    public bool isPaused { get; private set; } = false;
    public List<Door> doors;
    public List<Room> rooms;
    public List<Item> items;
    public string currentRoomId;
    public Room currentRoom;

    private InputParser inputParser;
    private Vocabulary vocabulary;
    [SerializeField] private TextController textController;
    [SerializeField] private TMP_InputField inputField;

    [SerializeField] private TextAsset doorsFile;
    [SerializeField] private TextAsset roomsFile;
    [SerializeField] private TextAsset itemsFile;
    [SerializeField] private RectTransform contentRectTransform;
    [SerializeField] private RectTransform pauseScreen;

    private void Start()
    {
        InitializeGame();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            TogglePauseScreen(isPaused);
        }
    }

    private void TogglePauseScreen(bool gameIsPaused)
    {
        if(gameIsPaused)
            ShowPauseScreen();
        else
            HidePauseScreen();
    }

    private void ShowPauseScreen()
    {
        DeactivateInputField();
        pauseScreen.gameObject.SetActive(true);
    }

    private void HidePauseScreen()
    {
        pauseScreen.gameObject.SetActive(false);
        ActivateInputField();
    }

    private void InitializeGame()
    {
        doors = GameContentLoader.LoadGameDoors(doorsFile);
        rooms = GameContentLoader.LoadGameRooms(roomsFile);
        items = GameContentLoader.LoadGameItems(itemsFile);


        inputParser = new InputParser();
        vocabulary = new Vocabulary(new List<string>() { "LOOK", "OPEN", "ENTER", "USE", "GO TO" }, new Dictionary<string, List<string>>());
        SetupRooms();

        GoToRoom(currentRoomId);

        isPaused = false;
        TogglePauseScreen(isPaused);
    }

    private void SetupRooms()
    {
        foreach (Room room in rooms)
        {
            room.SetupDoors(doors);
            room.SetupItems(items);
        }
    }

    public void GoToRoom(string roomId)
    {
        currentRoomId = roomId;
        currentRoom = rooms.Find(x => x.id == currentRoomId);
        ShowSelfText(currentRoom.selfDescription);
    }

    public void ParseInput(string input)
    {
        if(string.IsNullOrEmpty(input))
        {
            FinalizeTextShow();
            return;
        }

        if(input.ToUpper() == "QUITGAME")
        {
            Application.Quit();
        }

        VerbCheckResult verbResult = InputParser.HasVerb(input);

        //Room currentRoom = rooms.Find(x => x.id == currentRoomId);
        List<Door> doorsInRoom = doors.FindAll(x => currentRoom.doors.Contains(x));
        List<Item> itemsInRoom = currentRoom.items;

        TargetCheckResult targetResult = InputParser.HasTarget(input, currentRoom);
        if (targetResult.success && targetResult.target == null)
        {
            targetResult.target = currentRoom;
        }

        if (verbResult.success && targetResult.success)
        {
            bool canCombine = targetResult.target.validVerbs.HasFlag(verbResult.verb);
            string successText = canCombine ? "OK" : "NO";
            ShowAcknowledgementText($"{verbResult.verbString} {targetResult.target.name}? {successText}");

            if (canCombine)
            {
                if (targetResult.target is Door targetDoor)
                {
                    InteractWithDoor(targetDoor, verbResult, currentRoom);
                }
                else if (targetResult.target is Item targetItem)
                {
                    InteractWithItem(targetItem, verbResult);
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
                ShowAcknowledgementText($"{verbResult.verbString} around? OK");
                LookAroundRoom(currentRoom);
            }
            else
            {
                CommandNotUnderstood();
            }
        }
        else if (targetResult.success)
        {
            ShowAcknowledgementText($"Interested in {targetResult.target.name}? OK");
            ShowText($"{targetResult.target.shortDescription}");
        }
        else
        {
            CommandNotUnderstood();
        }
    }

    private void InteractWithRoom(Room currentRoom, VerbCheckResult verbResult)
    {
        if (verbResult.verb == Verb.LOOK)
        {
            string roomDescription = currentRoom.extendedDescription;
            ShowText($"{roomDescription}");
        }
    }

    private void LookAroundRoom(Room currentRoom)
    {
        string roomDescription = currentRoom.shortDescription;

        string doorsDesc = currentRoom.GetDoorsString();
        if (doorsDesc != string.Empty)
            roomDescription += doorsDesc;

        string itemsDesc = currentRoom.GetItemsString();
        if (itemsDesc != string.Empty)
            roomDescription += itemsDesc;

        ShowText($"{roomDescription}");
    }

    private void InteractWithItem(Item item, VerbCheckResult verbResult)
    {
        if (verbResult.verb == Verb.LOOK)
        {
            ShowText(item.extendedDescription);
        }
        else if (verbResult.verb == Verb.USE)
        {
            List<string> itemIdsToIntroduce = item.itemIdsToUnlock;
            foreach (string itemId in itemIdsToIntroduce)
            {
                Item itemToIntroduce = items.Find(x => x.id == itemId);
                currentRoom.AddItem(itemToIntroduce);
            }

            List<string> doorIdsToUnlock = item.doorIdsToUnlock;
            foreach (string doorId in doorIdsToUnlock)
            {
                doors.Find(x => x.id == doorId).locked = false;
            }
            currentRoom.RemoveItem(item);
            ShowText(item.useItemText);
        }
    }

    private void InteractWithDoor(Door targetDoor, VerbCheckResult verbResult, Room currentRoom)
    {
        if (verbResult.verb == Verb.ENTER)
        {
            GoThroughDoorResult passDoorResult = targetDoor.GoThroughDoor(currentRoom, rooms);
            if (passDoorResult.success)
            {
                GoToRoom(passDoorResult.newRoom.id);
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
        else if(verbResult.verb == Verb.GO_TO)
        {
            GoThroughDoorResult passDoorResult = targetDoor.GoThroughDoor(currentRoom, rooms);
            if (passDoorResult.success)
            {
                GoToRoom(passDoorResult.newRoom.id);
            }
        }
        else if (verbResult.verb == Verb.LOOK)
        {
            ShowText($"{targetDoor.extendedDescription}");
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

    private void CommandNotUnderstood()
    {
        ShowAcknowledgementText($"I don't understand what you want me to do");
    }

    private void ShowText(string text)
    {
        textController.AddText(text);
        FinalizeTextShow();
    }

    private void ShowAcknowledgementText(string text)
    {
        textController.AddAcknowledgementText(text);
        FinalizeTextShow();
    }

    public void ShowSelfText(string text)
    {
        textController.AddSelfText(text);
        FinalizeTextShow();
    }

    private void FinalizeTextShow()
    {
        inputField.text = "";
        ActivateInputField();

        contentRectTransform.SetLeft(10);
        contentRectTransform.SetRight(10);
    }

    private void ActivateInputField()
    {
        inputField.enabled = true;
        inputField.interactable = true;
        inputField.Select();
        inputField.ActivateInputField();
    }

    private void DeactivateInputField()
    {
        inputField.interactable = false;
        inputField.DeactivateInputField();
        inputField.enabled = false;
    }
}
