using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DamageTest : MonoBehaviour
{
    public bool isAlive = true;

    public int currentHealth; // Aktuelle Leben -- siehe public
    public int maxHealth = 100; // Maximale Leben -- siehe public

    public HealthSlider healthbar; // Referenz zur Lebensanzeige  -- siehe public

    void Start()
    {
        // Setze die Lebenspunkte auf das Maximum
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth); // Update der Lebensanzeige
    }

    // Funktion zum Zufügen von Schaden
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Schaden anwenden

        healthbar.Sethealth(currentHealth); // Lebensanzeige aktualisieren

        // Wenn das Leben auf 0 oder darunter fällt, zerstöre den Bot
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Zerstört den Bot, wenn die Lebenspunkte auf 0 fallen
    public void Die()
    {
        isAlive = false;
        Destroy(gameObject);
    }
}
