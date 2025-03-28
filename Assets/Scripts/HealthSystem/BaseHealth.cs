using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;
    public HealthSlider healthbar;
    public Boolean destructed = false;
    public GameObject gameOver;

    private void Start()
    {
        health = maxHealth;

        healthbar.SetMaxHealth(maxHealth);     //Referenz auf public void der healthbar
    }
    public void TakeDamage(int damage)    //Wie viel Damage soll die Base bekommen?
    {
        health -= damage;

        healthbar.Sethealth(health);        //Referenz auf public void der healthbar

        if (health <= 0 )
        {
            BaseDestruction();
        }
    }

    void Update()
    {
        healthbar.Sethealth(health);
        
    }
    public void BaseDestruction()
    {
        Destroy(gameObject);
        destructed = true;
        gameOver.SetActive(true);
        Time.timeScale = 0f;
    }

}
