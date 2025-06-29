using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public string levelName;
    public bool LevelLocked;
    public int LevelNumber;

    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        Image image = GetComponent<Image>();
        //LevelLocked = ProgressManager.Instance.IsLocked(LevelNumber);
        
        if (LevelLocked)
        {
            image.color = Color.gray;
        }

        button.onClick.AddListener(LoadLevel);
    }

    public void LoadLevel()
    {
        
        if (!LevelLocked)
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            UnityEngine.Debug.Log("LevelIsLocked at LevelSelector" + levelName);
        }
            
    }
}
