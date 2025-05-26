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
    private int nextText;
    private TMP_Text Story;

    void Start()
    {
        Story = 
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
    void OnNextClick() 
    {
        if (textnumber > 0)
        {
            nextText = textnumber + skip;
        }
        else
        {
            nextText = 0;
        }

        loadtext(nextText);
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

