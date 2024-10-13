using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDamage : MonoBehaviour
{
    public int damage = 10; // Schaden, den der Bot verursacht
    private BaseHealth baseHealth; // Referenz auf das BaseHealth-Skript

    public string targetObjectName = "Base"; // Name des Zielobjekts (z.B. die Base)

    private void OnCollisionEnter(Collision collision)
    {
        // Überprüfe, ob der kollidierte Gegenstand das Zielobjekt ist
        if (collision.gameObject.name == targetObjectName)
        {
            // Versuche das BaseHealth-Skript vom Zielobjekt zu bekommen
            baseHealth = collision.gameObject.GetComponent<BaseHealth>();

            // Wenn BaseHealth-Skript vorhanden, Schaden zufügen
            if (baseHealth != null)
            {
                baseHealth.TakeDamage(damage);
            }

            // Zerstöre den Bot nach dem Angriff
            Destroy(gameObject);
        }
    }
}

