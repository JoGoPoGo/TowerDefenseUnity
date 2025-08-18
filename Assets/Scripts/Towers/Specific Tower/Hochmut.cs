using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hochmut : Tower
{
    public float rangeMinimum;
    public GameObject projectile;
    public GameObject Lauf;

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
    protected override void Shoot()
    {
        updatingTarget = false;
        shootSound.Play();
        StartCoroutine(ShootAnimation(target));
        base.Shoot();
        updatingTarget = true;
    }

    IEnumerator ShootAnimation(GameObject currentTarget)
    {
        Vector3 targetPosition = currentTarget.transform.position;
        Vector3 startPosition = projectile.transform.position;
        float t = 0f;

        while (t < 1f)
        {
            projectile.transform.position = Vector3.Lerp(startPosition, targetPosition + new Vector3 (0, 2, 0), t);
            t = t + 4 * Time.deltaTime;
            yield return null;
        }

        projectile.transform.position = Lauf.transform.position; 
    }
}
