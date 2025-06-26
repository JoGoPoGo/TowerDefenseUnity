using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowStars : MonoBehaviour
{

    public GameObject Base;
    public TMP_Text display;
    public GameObject WinScreen;

    public int levelNumber;

    public static int collectedStars;

    bool savedStars = false;

    private BaseHealth baseSkript;
    // Start is called before the first frame update
    void Start()
    {
        baseSkript = Base.GetComponent<BaseHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (WinScreen.activeSelf && !savedStars)
        {
            ProgressManager.Instance.SaveStarsForLevel(levelNumber, baseSkript.receivedStars);
            display.text = "Stars: " + baseSkript.receivedStars + "\nTotal Stars:" + ProgressManager.Instance.GetTotalStars(levelNumber);

            collectedStars += baseSkript.receivedStars;

            savedStars = true;  // Prevent this from running again every frame
        }
    }
}
