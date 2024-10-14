using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDamage : MonoBehaviour
{
    public int damage = 10; 
    private BaseHealth baseHealth;

    private void Start()
    {
        baseHealth = GameObject.Find("Base2").GetComponent<BaseHealth>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.name == "Base2")
        {
            
            baseHealth = collision.gameObject.GetComponent<BaseHealth>();

            
            if (baseHealth != null)
            {
                baseHealth.TakeDamage(damage);
            }

            
            Destroy(gameObject);
        }
    }
}

