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
    
    public void AddText(string text)
    {
        var entry = Instantiate(textEntryPrefab, textContainer);
        entry.GetComponent<TextMeshProUGUI>().text = text;
        textsList.Add(entry);

        if(textsList.Count > maxTextsListCount)
        {
            GameObject lastObject = textsList[0];
            Destroy(lastObject);
            textsList.RemoveAt(0);
        }
    }

    public void AddAcknowledgementText(string text)
    {
        AddText(text);
        //acknowledgementText.text = text + 
        //    "\nWhat you want me to do next?";
    }
}
