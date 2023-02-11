using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetText(string textToShow, TextType textType)
    {
        text.text = textToShow;
        Color color = Color.white;

        switch (textType)
        {
            case TextType.SELF:
                ColorUtility.TryParseHtmlString("#E0C1B3", out color);
                break;
            case TextType.ACKNOWLEDGEMENT:
                color = Color.yellow;
                break;
            case TextType.NONE:
            case TextType.STANDARD:
                ColorUtility.TryParseHtmlString("#F2E3DC", out color);
                break;
            default:
                break;
        }

        text.color = color;
    }
}

public enum TextType
{
    NONE = 0,
    SELF = 1,
    ACKNOWLEDGEMENT = 2,
    STANDARD = 3
}
