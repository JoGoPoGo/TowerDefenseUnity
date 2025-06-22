using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    private static ProgressManager _instance;
    public static ProgressManager Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = FindObjectOfType<ProgressManager>();
                if(_instance == null)
                {
                    GameObject go = new GameObject("ProgressManager");
                    _instance = go.AddComponent<ProgressManager>();
                }
            }
            return _instance;
        }
    }

    public void UnlockLevel(int level)
    {
        PlayerPrefs.SetInt("LevelUnlocked_" + level, 1);
        PlayerPrefs.Save();
    }

    public void LockLevel(int level)
    {
        PlayerPrefs.SetInt("LevelUnlocked_" + level, 0);
        PlayerPrefs.Save();
    }

    public bool IsLocked(int level)
    {
        return PlayerPrefs.GetInt("LevelUnlocked_" + level, 0) == 1;
    }
    public void SaveUnlockedLevel(int level)
    {
        int current = PlayerPrefs.GetInt("unlockedLevel", 1);
        if (level > current)
        {
            PlayerPrefs.SetInt("unlockedLevel", 1);
            PlayerPrefs.Save();
        }
    }
    public int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt("unlockedLevel", 1);
    }

    public void SaveStarsForLevel(int levelIndex, int stars)
    {
        string key = $"level(levelIndex)Stars";
        int current = PlayerPrefs.GetInt(key, 0);
        if (stars > current)
        {
            PlayerPrefs.SetInt(key, stars);
            PlayerPrefs.Save();
        }
    }

    public int GetStarsForLevel(int levelIndex)
    {
        return PlayerPrefs.GetInt($"level(levelIndex)Stars", 0);
    }
    public int GetTotalStars(int numberOfLevels)
    {
        int totalStars = 0;
        for (int i = 1; i <= numberOfLevels; i++)
        {
            totalStars += GetStarsForLevel(i);
        }
        return totalStars;
    }
}
