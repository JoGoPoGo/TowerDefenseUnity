using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    public Slider slider;  // Referenz auf den Slider

    public Gradient gradient; //Farbe

    public Image fill;

    // Setze die maximale Gesundheit im Slider
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }

    // Aktualisiere die aktuelle Gesundheit im Slider
    public void Sethealth(int health)
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}