using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SwitchText : MonoBehaviour
{
    public Text[] textA;
    public int textnumber;
    public Button next;

    private Text currentText;

    void Start()
    {
        currentText = textA[textnumber];
        if (next != null)
        {
            next.onClick.AddListener(OnNextClick);
        }
    }
    void OnNextClick() // Blättert zur nächsten Seite
    {
        Text nextText = textA[textnumber + 1]; 
        currentText.gameObject.SetActive(false);
        nextText.gameObject.SetActive(true);
        textnumber++;
    }
    public void loadtext (int number)
    {
        Text loadingText = textA[number];
        foreach (Text txt in textA)
        {
            txt.gameObject.SetActive(false );
        }
        loadingText.gameObject.SetActive(true);
    }
}

