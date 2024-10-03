using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTest : MonoBehaviour
{
    public int currentHealth; //Aktuelle Leben -- siehe public
    public int maxHealth = 100; //leben -- siehe public
    

    public HealthSlider healthbar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            TakeDamage(20);
        }
        
    }
    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthbar.Sethealth(currentHealth);
    }
}
