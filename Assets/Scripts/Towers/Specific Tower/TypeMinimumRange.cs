using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeMinimumRange : Tower
{
    private bool updatingTarget = true;

    protected override void Update()
    {
        base.Update();
    }

    protected override void UpdateTarget()
    {
        // Sucht nach allen Gegnern mit dem Tag "Enemy"
        if (updatingTarget)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;

            // Finde den nächsten Gegner innerhalb der Reichweite
            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance && distanceToEnemy > rangeMinimum)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                    /*Vector3 dirToEnemy = (enemy.transform.position - transform.position).normalized;
                    RaycastHit hit;

                    if (!Physics.Raycast(transform.position, dirToEnemy, out hit, range, obstacleMask))
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = enemy;
                    } */// Hochmut betrifft diese Einschränkung zunächst nicht
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
}
