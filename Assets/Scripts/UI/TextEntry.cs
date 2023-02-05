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
                color = Color.grey;
                break;
            case TextType.ACKNOWLEDGEMENT:
                color = Color.yellow;
                break;
            case TextType.NONE:
            case TextType.STANDARD:
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
