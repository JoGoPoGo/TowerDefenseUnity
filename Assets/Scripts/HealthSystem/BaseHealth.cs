using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;
    public HealthSlider healthbar;

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
            Destroy(gameObject);
        }
    }

    void Update()
    {
        healthbar.Sethealth(health);
        
    }
}
