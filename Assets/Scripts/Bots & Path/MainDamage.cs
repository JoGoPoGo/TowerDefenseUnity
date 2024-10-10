using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDamage : MonoBehaviour
{
    public int damage = 10; // Schaden, den der Bot verursacht
    private BaseHealth baseHealth; // Referenz auf das BaseHealth-Skript

    // Start is called before the first frame update
    void Start()
    {
        // Suche das Objekt mit dem Tag "Base" in der Szene
        GameObject baseObject = GameObject.FindWithTag("Base");
        if (baseObject != null)
        {
            // Hole das BaseHealth-Skript vom Basisobjekt
            baseHealth = baseObject.GetComponent<BaseHealth>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Hier kannst du andere Dinge in deinem Update machen, falls nötig
    }

    private void OnCollisionEnter(Collision col)
    {
        // Überprüfe, ob der Bot mit der Basis kollidiert ist
        if (col.gameObject.CompareTag("Base"))
        {
            // Schaden zufügen und den Bot zerstören
            baseHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

