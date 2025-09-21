using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowStars : MonoBehaviour
{

    public GameObject Base;
    public GameObject WinScreen;

    public int levelNumber;
    public int totalStars;
    public int displayStars;

    bool savedStars = false;

    private BaseHealth baseSkript;
    // Start is called before the first frame update
    void Start()
    {
        baseSkript = Base.GetComponent<BaseHealth>();
        displayStars = ProgressManager.Instance.GetCollectedStars();
    }

    // Update is called once per frame
    void Update()
    {
        if (WinScreen.activeSelf && !savedStars)
        {
            ProgressManager.Instance.SaveStarsForLevel(levelNumber, baseSkript.receivedStars);
            displayStars = ProgressManager.Instance.GetCollectedStars();

            totalStars = baseSkript.receivedStars;

            savedStars = true;  // Prevent this from running again every frame
        }
    }
}
