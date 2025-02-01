using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DamageTest : MonoBehaviour
{
    public bool isAlive = true;

    public int currentHealth; // Aktuelle Leben -- siehe public
    public int maxHealth = 100; // Maximale Leben -- siehe public
    public float speed = 1f;
    public int reward = 0;

    public HealthSlider healthbar; // Referenz zur Lebensanzeige  -- siehe public

    public bool isLast;

    private GameManager gameManager;

    void Start()
    {
        // Setze die Lebenspunkte auf das Maximum
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth); // Update der Lebensanzeige
        gameManager = FindObjectOfType<GameManager>();
    }

    // Funktion zum Zufügen von Schaden
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Schaden anwenden

        healthbar.Sethealth(currentHealth); // Lebensanzeige aktualisieren

        // Wenn das Leben auf 0 oder darunter fällt, zerstöre den Bot
        if (currentHealth <= 0)
        {
            Die(false);
        }
    }

    // Zerstört den Bot, wenn die Lebenspunkte auf 0 fallen
    public void Die(bool didDamage)
    {
        if (!didDamage)
        {
            gameManager.AddCredits(reward);
        }
        if (IsOnlyEnemy() && isLast)
        {
            SceneManager.LoadScene("LevelAuswahl");   //Auskommentieren, falls es zu unerwünschten Szenenwechsel kommt
        }
        isAlive = false;
        Destroy(gameObject);
    }
    public bool IsOnlyEnemy()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length == 1; //true, wenn es keine weiteren Gegner gibt.
    }
}
