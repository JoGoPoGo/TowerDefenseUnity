using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLock : MonoBehaviour
{
    public int currentMaxLevel;

    private int stars;
    LevelSelector[] allDisplays;

    void Start()
    {
        stars = ProgressManager.Instance.GetCollectedStars();
        Debug.Log(stars);

        allDisplays = FindObjectsOfType<LevelSelector>();

        LevelsToLock();
    }

    public void LevelsToLock()
    { 

        int smallestLockedLevel = 2;

        if (stars >= 1)
            smallestLockedLevel = 3;
        if (stars >= 2)
            smallestLockedLevel = 4;
        if (stars >= 4)
            smallestLockedLevel = 5;
        if (stars >= 6)
            smallestLockedLevel = 6;
        if (stars >= 7)
            smallestLockedLevel = 7; 
        if (stars >= 8)
            smallestLockedLevel = 8;
        if (stars >= 9) 
            smallestLockedLevel = 9;
        if (stars >= 27)
            smallestLockedLevel = 10;

        if(currentMaxLevel < smallestLockedLevel)
        {
            currentMaxLevel = smallestLockedLevel;
            Debug.LogWarning("Zu niedriger currentMaxlevel at LevelLock");
        }
            

        for (int i=smallestLockedLevel; i <= currentMaxLevel; i++)
        {
            lockLevel(i);
        }
    }
    void lockLevel(int level)
    {
        ProgressManager.Instance.LockLevel(level);
        foreach(LevelSelector s in allDisplays)
        {
            if(s.LevelNumber == level)
            {
                s.LevelLocked = true;
            }
        } 
    }
}
