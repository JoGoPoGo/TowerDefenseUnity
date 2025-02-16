using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SwitchText : MonoBehaviour
{
    public TMP_Text[] textA;
    public int textnumber;
    public Button next;
    public int skip = 1;

    private TMP_Text startText;
    private int currentIndex;

    void Start()
    {
        startText = textA[textnumber];
        if (next != null)
        {
            next.onClick.AddListener(OnNextClick);
        }
        foreach (TMP_Text txt in textA)
        {
            txt.gameObject.SetActive(false);
        }
        startText.gameObject.SetActive(true);
    }
    void OnNextClick() // Blättert zur nächsten Seite
    {
        int nextIndex = textnumber + skip; // berechnet den index des nächsten Textes
        Debug.Log(nextIndex);
        if (nextIndex >= 0 && nextIndex <= textA.Length)
        {
            TMP_Text nextText = textA[textnumber + skip];
            startText.gameObject.SetActive(false);
            if (startText.isActiveAndEnabled)
            {
                Debug.Log("currentText ist aktiv");
            }
            nextText.gameObject.SetActive(true);
            textnumber = nextIndex;
        }
        Debug.Log(nextIndex);
    }
    public void loadtext (int number)
    {
        TMP_Text loadingText = textA[number];
        foreach (TMP_Text txt in textA)
        {
            txt.gameObject.SetActive(false );
        }
        loadingText.gameObject.SetActive(true);
    }
}

