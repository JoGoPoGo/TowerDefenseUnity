using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
