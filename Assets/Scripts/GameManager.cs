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
    public bool isInEndScreen { get; private set; } = false;
    public bool musicOn { get; private set; } = true;
    private bool isInGame = false;
    private GameState gameState;

    public List<Door> doors;
    public List<Room> rooms;
    public List<Item> items;
    public string currentRoomId;
    public string startRoomId;
    public string endRoomId;
    public Room currentRoom;

    private InputParser inputParser;
    private Vocabulary vocabulary;
    [SerializeField] private List<Sprite> roomImageSprites;
    [SerializeField] private TextController textController;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private AudioSource gameMusic;

    [SerializeField] private TextAsset doorsFile;
    [SerializeField] private TextAsset roomsFile;
    [SerializeField] private TextAsset itemsFile;
    [SerializeField] private RectTransform contentRectTransform;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject inGameImage;

    private void Start()
    {
        isPaused = false;
        SetPauseScreen(isPaused);

        isInEndScreen = false;
        SetEndScreen(isInEndScreen);

        gameState = GameState.IN_START_SCREEN;
    }

    private void Update()
    {
        if(isInGame && Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            SetPauseScreen(isPaused);
        }
    }

    private void SetEndScreen(bool endScreenOn)
    {
        endScreen.SetActive(endScreenOn);
        if(endScreenOn)
        {
            isInGame = false;
            inputField.onEndEdit.RemoveAllListeners();
            inputField.onEndEdit.AddListener(ParseMenuInput);
            StopMusic();
        }
    }

    private void SetPauseScreen(bool gameIsPaused)
    {
        if(gameIsPaused)
            ShowPauseScreen();
        else
            HidePauseScreen();
    }

    private void ShowPauseScreen()
    {
        DeactivateInputField();
        pauseScreen.SetActive(true);
    }

    private void HidePauseScreen()
    {
        pauseScreen.SetActive(false);
        ActivateInputField();
    }

    private void ToggleMusic()
    {
        musicOn = !musicOn;

        if (musicOn)
            StartMusic();
        else
            StopMusic();
    }

    private void InitializeGame()
    {
        doors = GameContentLoader.LoadGameDoors(doorsFile);
        rooms = GameContentLoader.LoadGameRooms(roomsFile);
        items = GameContentLoader.LoadGameItems(itemsFile);

        inputParser = new InputParser();
        vocabulary = new Vocabulary(new List<string>() { "LOOK", "OPEN", "ENTER", "USE", "GO TO" }, new Dictionary<string, List<string>>());
        SetupRooms();

        inputField.onEndEdit.RemoveAllListeners();
        inputField.onEndEdit.AddListener(ParseInput);

        textController.RemoveAllEntries();

        menuScreen.SetActive(false);

        GoToRoom(startRoomId);

        isPaused = false;
        SetPauseScreen(isPaused);

        isInEndScreen = false;
        SetEndScreen(isInEndScreen);

        inGameImage.SetActive(true);

        musicOn = true;
        StartMusic();

        isInGame = true;

        gameState = GameState.EXPECTING_PLAYER_TEXT_ADVANCE;
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

        inGameImage.GetComponent<Image>().sprite = roomImageSprites[currentRoom.imageSpriteIndex];

        if(currentRoomId == endRoomId)
        {
            TriggerEndSequence();
        }
    }

    private void TriggerEndSequence()
    {
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        DeactivateInputField();
        yield return new WaitForSeconds(2);
        SetEndScreen(true);
        ActivateInputField();
    }

    public void ParseMenuInput(string input)
    {
        if (input.ToUpper() == "QUITGAME")
        {
            Application.Quit();
            return;
        }

        if (input.ToUpper() == "START")
        {
            InitializeGame();
        }

        ResetInputField();
    }

    public void ParseInput(string input)
    {
        if(string.IsNullOrEmpty(input))
        {
            ResetInputField();
            return;
        }

        if(input.ToUpper() == "QUITGAME")
        {
            Application.Quit();
            return;
        }

        if(input.ToUpper() == "MUSIC")
        {
            ToggleMusic();
            ResetInputField();
            return;
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
            ShowStandardText($"{targetResult.target.shortDescription}");
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
            ShowStandardText($"{roomDescription}");
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

        ShowStandardText($"{roomDescription}");
    }

    private void InteractWithItem(Item item, VerbCheckResult verbResult)
    {
        if (verbResult.verb == Verb.LOOK)
        {
            ShowStandardText(item.extendedDescription);
        }
        else if (verbResult.verb == Verb.USE || verbResult.verb == Verb.OPEN)
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
            ShowStandardText(item.useItemText);
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
                    ShowStandardText($"The door is locked");
                }
                else if (passDoorResult.closed)
                {
                    ShowStandardText($"The door is closed");
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
            ShowStandardText($"{targetDoor.extendedDescription}");
        }
        else if (verbResult.verb == Verb.OPEN)
        {
            var result = targetDoor.OpenDoor();
            if (result.success)
            {
                ShowStandardText($"The door is now open.");
            }
            else
            {
                if (result.locked)
                {
                    ShowStandardText($"The door is locked.");
                }
                else if (result.alreadyOpen)
                {
                    ShowStandardText($"The door is already open.");
                }
            }
        }
    }

    private void CommandNotUnderstood()
    {
        ShowAcknowledgementText($"I don't understand what you want me to do");
    }

    private void ShowStandardText(string text) => ShowText(text, TextType.STANDARD);

    private void ShowAcknowledgementText(string text) => ShowText(text, TextType.ACKNOWLEDGEMENT);

    private void ShowSelfText(string text) => ShowText(text, TextType.SELF);

    private void ShowText(string text, TextType textType)
    {
        textController.SetCurrentDescriptionsText(new List<string> { text });
        textController.ShowNextText(textType);
        if (textController.HasMoreText())
        {

        }
        else
        {
            ResetInputField();
        }
    }

    private void ResetInputField()
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

    private void StartMusic() => gameMusic.Play();
    private void StopMusic() => gameMusic.Stop();
}

public enum GameState
{
    NONE = 0,
    IN_START_SCREEN = 1,
    IN_END_SCREEN = 2,
    IN_PAUSE_SCREEN = 3,
    EXPECTING_PLAYER_TEXT = 4,
    EXPECTING_PLAYER_TEXT_ADVANCE = 5,
    OUTPUTING_TEXT = 6,
}