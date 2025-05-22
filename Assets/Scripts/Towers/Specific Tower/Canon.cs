using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : Tower
{
    public int rangeDegrees;

    private int startDirection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void UpdateTarget()   
    {
        // Sucht nach allen Gegnern mit dem Tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        // Finde den nächsten Gegner innerhalb der Reichweite
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        // Wenn ein Gegner gefunden wurde, setze ihn als Ziel
        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy;
        }
        else
        {
            target = null; // Kein Gegner in Reichweite
        }
    }
}
