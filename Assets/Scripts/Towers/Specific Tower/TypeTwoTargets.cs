using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeTwoTargets : Tower
{
    public GameObject targetSnd;

    public GameObject canonTargetOne;
    public GameObject canonTargetTwo;

    public GameObject projectileOne;
    public GameObject projectileTwo;

    [Header("Changeables")]

    public float weakerPercentage = 0;

    protected override void Update()
    {
        if (gameObject.CompareTag("Tower") && dictionaryActivater)
        {
            Vector2Int posi = new Vector2Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));
            OccupyPositionsInCircle(posi, spawnCancelRadius);
            dictionaryActivater = false;
        }

        if (UpdateCounter >= updateTargetIntervall)
        {
            UpdateCounter = 0;
        }

        if (UpdateCounter == 0 && gameObject.CompareTag("Tower"))
        {
            UpdateTarget();
        }

        UpdateCounter += Time.deltaTime;

        if (target == null)   //führt nichts aus, wenn kein Ziel gefunden wurde
            return;

        RotateCanons();

        // Wenn die Zeit zum Schießen gekommen ist, wird geschossen
        if (fireCountdown <= 0f)
        {
            if (gameObject.CompareTag("Tower"))
            {
                if (!spawnScript.spawned)
                {
                    if (shootSound != null)
                        audioSource.PlayOneShot(shootSound);
                }
                Shoot();
            }

            fireCountdown = 1f / fireRate; // Setze den Timer für den nächsten Schuss
        }

        fireCountdown -= Time.deltaTime;
    }
    protected override void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        float shortestDistance = Mathf.Infinity;
        float sndShortestDistance = Mathf.Infinity;

        GameObject nearestEnemy = null;
        GameObject sndNearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            Vector3 direction = (enemy.transform.position + new Vector3(0, 2f, 0)) - transform.position;
            float distance = direction.magnitude;

            if (distance > range)
                continue;

            // Sichtprüfung
            if (Physics.Raycast(transform.position + new Vector3(0, 2f, 0), direction.normalized, distance, obstacleMask))
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
    }

    protected override void Shoot()
    {
        Debug.Log("Shoooot");

        if (spawnScript.spawned)
            return;

        if (target != null)
        {
            GameObject proOne = Instantiate(
                projectilePrefab,
                projectileOne.transform.position,
                projectileOne.transform.rotation
            );

            proOne.transform.SetParent(projectileOne.transform, true);
            StartCoroutine(ProjectileAnimation(proOne, target));

        }

        if (targetSnd != null)
        {
            GameObject proTwo = Instantiate(
                projectilePrefab,
                projectileTwo.transform.position,
                projectileTwo.transform.rotation
            );

            proTwo.transform.SetParent(projectileTwo.transform, true);
            StartCoroutine(ProjectileAnimation(proTwo, targetSnd));

        }
    }
    void RotateCanons()
    {
        if (target != null)
        {
            RotateCanon(canonTargetOne.transform, target.transform);
        }

        if (targetSnd != null)
        {
            RotateCanon(canonTargetTwo.transform, targetSnd.transform);
        }
    }

    void RotateCanon(Transform canon, Transform targetTransform)
    {
        Vector3 direction = targetTransform.position - canon.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        Quaternion smoothedRotation = Quaternion.Lerp(
            canon.rotation,
            lookRotation,
            Time.deltaTime * turnSpeed
        );

        canon.rotation = Quaternion.Euler(0f, smoothedRotation.eulerAngles.y, 0f);
    }
}
