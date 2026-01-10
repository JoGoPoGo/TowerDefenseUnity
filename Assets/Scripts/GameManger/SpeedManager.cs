using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedManager : MonoBehaviour
{
    public TMP_Text timeText;

    public void NormalSpeed()
    {
        Time.timeScale = 1f;
        timeText.text = "x" + Time.timeScale.ToString();
    }

    public void Faster()
    {
        if(Time.timeScale < 4f)
        {
            Time.timeScale *= 2f;
            timeText.text = "x" + Time.timeScale.ToString();
        }
        else
        {
            Debug.LogWarning("TimeScale is too high");
        }
    }

    public void Slower()
    {
        if(Time.timeScale > 0.125f)
        {
            Time.timeScale *= 0.5f;
            timeText.text = "x" + Time.timeScale.ToString();
        }
        else
        {
            Debug.LogWarning("TimeScale is too low");
        }

    }
}
