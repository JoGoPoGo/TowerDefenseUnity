using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BotHealth : MonoBehaviour
{
    public int maxHealth = 100; //maximle Gesundheit vom Bot -- siehe public
    private int currentHealth;
    private HealthSlider healthbar;

    // Initialisiert die Gesundheit und setzt die Healthbar
    public void InitializeHealth(int health, HealthSlider slider)
    {
        maxHealth = health;
        currentHealth = maxHealth;
        healthbar = slider;
        healthbar.SetMaxHealth(maxHealth);
    }

    // Funktion, um Schaden zu nehmen
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        healthbar.Sethealth(currentHealth);

        //Zerstört bot wenn tod
        if (currentHealth <= 0)
        {
            Destroy(gameObject);  // Zerstört den Bot
        }
    }
}
