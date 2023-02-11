using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    [SerializeField] private GameObject textEntryPrefab;
    [SerializeField] private RectTransform textContainer;
    [SerializeField] private TextMeshProUGUI acknowledgementText;
    [SerializeField] private int maxTextsListCount = 50;
    private List<GameObject> textsList = new List<GameObject>();
    private List<string> currentDescriptionTexts = new List<string>();
    private int currentDescriptionTextsCounter = 0;
    private TextType currentTextType;

    public void SetCurrentDescriptionsText(List<string> texts, TextType textType)
    {
        currentDescriptionTexts = texts;
        currentDescriptionTextsCounter = 0;
        currentTextType = textType;
    }

    public bool HasMoreText()
    {
        return currentDescriptionTextsCounter < currentDescriptionTexts.Count;
    }

    public void ShowNextText()
    {
        string text = currentDescriptionTexts[currentDescriptionTextsCounter];
        var entry = Instantiate(textEntryPrefab, textContainer);
        entry.GetComponent<TextEntry>().SetText(text, currentTextType);
        textsList.Add(entry);

        if (textsList.Count > maxTextsListCount)
        {
            RemoveFirstEntry();
        }

        currentDescriptionTextsCounter++;
    }

    private void RemoveFirstEntry()
    {
        GameObject lastObject = textsList[0];
        Destroy(lastObject);
        textsList.RemoveAt(0);
    }

    public void RemoveAllEntries()
    {
        while (textsList.Count > 0)
        {
            RemoveFirstEntry();
        }
    }
}