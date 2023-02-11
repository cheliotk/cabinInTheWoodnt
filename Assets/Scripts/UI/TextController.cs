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

    public void SetCurrentDescriptionsText(List<string> texts)
    {
        currentDescriptionTexts = texts;
        currentDescriptionTextsCounter = 0;
    }

    public bool HasMoreText()
    {
        return currentDescriptionTextsCounter < currentDescriptionTexts.Count;
    }

    public void ShowNextText(TextType textType)
    {
        string text = currentDescriptionTexts[currentDescriptionTextsCounter];
        var entry = Instantiate(textEntryPrefab, textContainer);
        entry.GetComponent<TextEntry>().SetText(text, textType);
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