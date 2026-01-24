using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeTwoTargets : Tower
{
    public GameObject targetSnd;
    private DamageTest damageScriptSnd;


    protected override void UpdateTarget()
    {
        Debug.Log("UpdateTarget");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        float shortestDistance = Mathf.Infinity;
        float sndShortestDistance = Mathf.Infinity;

        GameObject nearestEnemy = null;
        GameObject sndNearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            Vector3 direction = enemy.transform.position - transform.position;
            float distance = direction.magnitude;

            if (distance > range)
                continue;

            // Sichtprüfung
            if (Physics.Raycast(transform.position, direction.normalized, distance, obstacleMask))
                continue;

            if (distance < shortestDistance)
            {
                // alter nearest wird zweitnächster
                sndShortestDistance = shortestDistance;
                sndNearestEnemy = nearestEnemy;

                shortestDistance = distance;
                nearestEnemy = enemy;
            }
            else if (distance < sndShortestDistance)
            {
                sndShortestDistance = distance;
                sndNearestEnemy = enemy;
            }
        }

        target = nearestEnemy;
        targetSnd = sndNearestEnemy; 
        if(target == null)
            target = sndNearestEnemy;
    }

    protected override void Shoot()
    {
        Debug.Log("Shoooot");
        if (!spawnScript.spawned)
        {
            damageScript = target.GetComponent<DamageTest>();
            damageScript.TakeDamage(damageAmount);
            damageScriptSnd = targetSnd.GetComponent<DamageTest>();
            damageScriptSnd.TakeDamage(damageAmount);
        }
    }

}
