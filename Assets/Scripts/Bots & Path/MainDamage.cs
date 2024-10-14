using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDamage : MonoBehaviour
{
    public int damage = 10; 
    private BaseHealth baseHealth; 

    public string targetObjectName = "Base2"; 

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.name == targetObjectName)
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

