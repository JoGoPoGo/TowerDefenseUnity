using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseHealth : MonoBehaviour
{

    public int maxHealth = 100;
    public int health;

    public int receivedStars = 3;


    public HealthSlider healthbar;
    public Boolean destructed = false;

    public TMP_Text UIhealth;

    [Header("UI Referenzen")]
    public GameObject gameOverScreen;   // Das Fenster mit Buttons & Text
    public CanvasGroup backgroundFade;

    private void Start()
    {
        health = maxHealth;

        healthbar.SetMaxHealth(maxHealth);     //Referenz auf public void der healthbar
    }
    public void TakeDamage(int damage)    //Wie viel Damage soll die Base bekommen?
    {
        health -= damage;

        if(receivedStars == 3)    //Wenn das erste Mal Schaden genommen wird, erhält man maximal 2 Sterne
        {
            receivedStars = 2;
        }

        if(health < (maxHealth/3) * 2)
        {
            receivedStars = 1;
        }
        if(health < (maxHealth / 3))
        {
            receivedStars = 0;
        }

        healthbar.Sethealth(health);        //Referenz auf public void der healthbar

        if (health <= 0 )
        {
            BaseDestruction();
        }
    }

    void Update()
    {
        healthbar.Sethealth(health);
        UIhealth.text = "Lebenspunkte: " + health.ToString() + ("/") + maxHealth.ToString();
    }
    public void BaseDestruction()
    {
        if (destructed) return;
        destructed = true;
        gameOverScreen.SetActive(true);
        if (backgroundFade != null)
        {
            backgroundFade.gameObject.SetActive(true); // Sicherstellen, dass es an ist
            backgroundFade.alpha = 0f;                 // Start: Unsichtbar
            backgroundFade.DOFade(1f, 1f).SetUpdate(true);
        }
        Time.timeScale = 0f;
    }

}
