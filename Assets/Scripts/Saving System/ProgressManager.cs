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
    public void Reset()
    {
        PlayerPrefs.SetInt("collectedStars", 0);
        for (int f = 10; f > 0; f--)
        {
            int levelIndex = f;
            string key = $"level{levelIndex}_Stars";
            PlayerPrefs.SetInt(key, 0);
        }
        PlayerPrefs.Save();
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
            PlayerPrefs.SetInt("unlockedLevel", level);
            PlayerPrefs.Save();
        }
    }
    public int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt("unlockedLevel", 1);
    }

    public void SaveStarsForLevel(int levelIndex, int stars)
    {
        string key = $"level{levelIndex}_Stars";
        Debug.Log(key + "at Progress Manager_61");
        int current = PlayerPrefs.GetInt(key, 0);

        if (stars > current)
        {
            SaveCollectedStars(-current);
            PlayerPrefs.SetInt(key, stars);
            SaveCollectedStars(stars);
            PlayerPrefs.Save();
        }
    }

    public int GetStarsForLevel(int levelIndex)
    {
        return PlayerPrefs.GetInt($"level{levelIndex}_Stars", 0);
    }

    public void SaveCollectedStars(int plus)
    {
        int current = PlayerPrefs.GetInt("collectedStars", 0);
        int updated = current + plus;
        PlayerPrefs.SetInt("collectedStars", updated);
        PlayerPrefs.Save();
    }
    public int GetCollectedStars()
    {
        return PlayerPrefs.GetInt("collectedStars", 0);
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
