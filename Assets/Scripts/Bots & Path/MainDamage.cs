using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDamage : MonoBehaviour
{
    public int damage = 10; // Schaden, den der Bot verursacht
    private BaseHealth baseHealth; // Referenz auf das BaseHealth-Skript

    public string targetObjectName = "Base2";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == targetObjectName)
        {
            baseHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
    }
}

