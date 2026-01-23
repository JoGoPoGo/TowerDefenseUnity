using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeTwoTargets : Tower
{
    public GameObject targetSnd;



    protected override void UpdateTarget()   // protected für Schussfunktionen und Animationen
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = range; // Nur Gegner **innerhalb der Reichweite** sind relevant
        float sndShortestDistance = range;
        GameObject nearestEnemy = null;
        GameObject sndNearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            Vector3 direction = enemy.transform.position - transform.position;
            float distance = direction.magnitude;
            if(distance <= sndShortestDistance && distance > shortestDistance)
            {
                if (!Physics.Raycast(transform.position, direction.normalized, distance, obstacleMask))
                {
                    sndShortestDistance = distance;
                    sndNearestEnemy = enemy;
                }
            }

            if (distance <= shortestDistance)
            {
                // Sichtprüfung mit Raycast (verhindert Schüsse durch Mauern o.ä.)
                if (!Physics.Raycast(transform.position, direction.normalized, distance, obstacleMask))
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }

        target = nearestEnemy;
    }

}
