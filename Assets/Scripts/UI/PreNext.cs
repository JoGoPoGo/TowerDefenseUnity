using System.Collections.Generic;
using UnityEngine;

public class PreNext : MonoBehaviour
{
    public List<GameObject> levelSections; // z.B. Section1, Section2, Section3
    private int currentIndex = 0;

    void Start()
    {
        ShowSection(currentIndex); // Zeige den ersten Abschnitt
    }

    public void NextSection()
    {
        if (currentIndex < levelSections.Count - 1)
        {
            currentIndex++;
            ShowSection(currentIndex);
        }
    }

    public void PreviousSection()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowSection(currentIndex);
        }
    }

    private void ShowSection(int index)
    {
        for (int i = 0; i < levelSections.Count; i++)
        {
            levelSections[i].SetActive(i == index); // nur der aktuelle Abschnitt ist aktiv
        }
    }
}

