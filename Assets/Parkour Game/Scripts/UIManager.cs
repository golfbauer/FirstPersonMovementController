using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public TMP_Text Talker;

    private Coroutine displayingText;
    void Start()
    {
        Talker.text = "";
    }

    public void DisplayText(string text, float displayTime)
    {
        if (text == "" || Talker.text == text || displayTime == 0)
        {
            return;
        }

        if (displayingText != null)
        {
            StopCoroutine(displayingText);
        }

        displayingText = StartCoroutine(InstantDisplayForSeconds(text, displayTime));
    }

    IEnumerator InstantDisplayForSeconds(string text, float displayTime)
    {
        Talker.text = text;
        yield return new WaitForSeconds(displayTime);
        Talker.text = "";
        
    }

}
