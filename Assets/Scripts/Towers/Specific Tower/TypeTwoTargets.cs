using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponSlot
{
    public GameObject canonTarget;
    public GameObject projectileSpawn;
}

public class TypeTwoTargets : Tower
{
    [Header("Weapon Slots")]
    public List<WeaponSlot> weaponSlots = new List<WeaponSlot>();

    [Header("Changeables")]
    public float weakerPercentage = 0;

    private List<GameObject> currentTargets = new List<GameObject>();

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

        if (currentTargets.Count == 0 || currentTargets[0] == null)
            return;

        RotateCanons();

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

            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    protected override void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        // Nur gültige Slots zählen
        List<WeaponSlot> validSlots = new List<WeaponSlot>();
        foreach (var slot in weaponSlots)
        {
            if (slot != null && slot.canonTarget != null && slot.projectileSpawn != null)
            {
                validSlots.Add(slot);
            }
        }

        int targetCount = validSlots.Count;

        currentTargets.Clear();

        if (targetCount == 0)
        {
            target = null;
            return;
        }

        // Liste möglicher Gegner mit Distanz
        List<(GameObject enemy, float distance)> validEnemies = new List<(GameObject, float)>();

        foreach (GameObject enemy in enemies)
        {
            Vector3 direction = (enemy.transform.position + new Vector3(0, 2f, 0)) - transform.position;
            float distance = direction.magnitude;

            if (distance > range)
                continue;

            // Sichtprüfung
            if (Physics.Raycast(transform.position + new Vector3(0, 2f, 0), direction.normalized, distance, obstacleMask))
                continue;

            validEnemies.Add((enemy, distance));
        }

        // Nach Distanz sortieren
        validEnemies.Sort((a, b) => a.distance.CompareTo(b.distance));

        // Die N nächsten nehmen
        for (int i = 0; i < Mathf.Min(targetCount, validEnemies.Count); i++)
        {
            currentTargets.Add(validEnemies[i].enemy);
        }

        // Hauptziel wie bisher beibehalten
        target = currentTargets.Count > 0 ? currentTargets[0] : null;
    }

    protected override void Shoot()
    {
        Debug.Log("Shoooot");

        if (spawnScript.spawned)
            return;

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            if (i >= currentTargets.Count)
                break;

            WeaponSlot slot = weaponSlots[i];
            GameObject currentTarget = currentTargets[i];

            if (slot == null || slot.projectileSpawn == null || currentTarget == null)
                continue;

            GameObject projectile = Instantiate(
                projectilePrefab,
                slot.projectileSpawn.transform.position,
                slot.projectileSpawn.transform.rotation
            );

            projectile.transform.SetParent(slot.projectileSpawn.transform, true);
            StartCoroutine(ProjectileAnimation(projectile, currentTarget));
        }
    }

    void RotateCanons()
    {
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            if (i >= currentTargets.Count)
                break;

            WeaponSlot slot = weaponSlots[i];
            GameObject currentTarget = currentTargets[i];

            if (slot == null || slot.canonTarget == null || currentTarget == null)
                continue;

            RotateCanon(slot.canonTarget.transform, currentTarget.transform);
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
