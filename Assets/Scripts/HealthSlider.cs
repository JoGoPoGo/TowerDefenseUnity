using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    public Slider slider;  // Referenz auf den Slider

    // Setze die maximale Gesundheit im Slider
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    // Aktualisiere die aktuelle Gesundheit im Slider
    public void Sethealth(int health)
    {
        slider.value = health;
    }
}
