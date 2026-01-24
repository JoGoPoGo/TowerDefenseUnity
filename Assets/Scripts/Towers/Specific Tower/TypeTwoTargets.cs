using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeTwoTargets : Tower
{
    public GameObject targetSnd;
    private DamageTest damageScriptSnd;

    public GameObject canonTargetOne;
    public GameObject canonTargetTwo;

    public GameObject projectileOne;
    public GameObject projectileTwo;

    public GameObject projectilePrefab;

    [Header("Changeables")]

    public float weakerPercentage;

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
                if (shootSound != null)
                    shootSound.Play();
                Shoot();
            }

            fireCountdown = 1f / fireRate; // Setze den Timer für den nächsten Schuss
        }

        fireCountdown -= Time.deltaTime;
    }
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
    }

    protected override void Shoot()
    {
        Debug.Log("Shoooot");

        if (spawnScript.spawned)
            return;

        if (target != null)
        {
            GameObject proOne = Instantiate (projectilePrefab, projectileOne.transform);
            StartCoroutine(ProjectileAnimation(proOne, target));

        }

        if (targetSnd != null)
        {
            GameObject proTwo = Instantiate(projectilePrefab, projectileTwo.transform);
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
    IEnumerator ProjectileAnimation(GameObject projectile, GameObject enemy)
    {
        float duration = 0.4f;
        float t = 0f;
        Vector3 startPos = projectile.transform.position;
        Vector3 targetPos = enemy.transform.position;
        float yPosTarget = targetPos.y;
        float yPosCurrent = startPos.y;
        targetPos.y = startPos.y;

        float subtract = 0f;

        while (t < duration)
        {
            yPosCurrent -= subtract;
            subtract += (startPos.y - yPosTarget) / Sum0ToN((int)Mathf.Round(duration / Time.deltaTime));
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t/duration);
            currentPos.y = yPosCurrent;
            projectile.transform.localScale += new Vector3(0, 0, 0.05f);
            projectile.transform.position = currentPos;
            t += Time.deltaTime;
            yield return null;
        }

        damageScript = target.GetComponent<DamageTest>();
        if (damageScript != null)
            damageScript.TakeDamage(damageAmount);

        Destroy(projectile);
    }
    int Sum0ToN(int n)
    {
        return n * (n + 1) / 2;
    }
}
