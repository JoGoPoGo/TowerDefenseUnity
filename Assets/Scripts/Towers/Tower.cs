using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class Tower : MonoBehaviour
{
    public SpawnOnMouseClick spawnScript; // Reference to the SpawnOnMouseClick script
    public GameObject target;           // Das aktuelle Ziel des Turms
    private DamageTest damageScript;   // DamageTest von Target

    public string enemyTag = "Enemy";  // Der Tag der Gegner (z.B. "Enemy")

    public GameObject projectilePrefab; // Projektil, das der Turm abfeuert
    public Transform firePoint;        // Ort, von dem aus das Projektil geschossen wird

    public int price;
    public float fireRate = 1f;        // Schussfrequenz
    public float bulletSpeed = 10f;     //Kugelgeschwindigkeit
    public float range = 15f;          // Reichweite des Turms
    public int damageAmount = 50;       //Schaden
    public float spawnCancelRadius = 10f;        // Kein Weiterer turm in diesem Bereich
   
    private float fireCountdown = 0f; 
    

    public float turnSpeed = 10f;      // Geschwindigkeit, mit der der Turm sich dreht

    public GameObject canon;

    public float recoilSpeed = 0.1f;
    public float recoilDistance = 0.2f;

    private int level = 1;  // Turm-Level beginnt bei 1

    void Start()
    {
        GameObject spawnHandler = GameObject.Find("SpawnHandler");
        spawnScript = spawnHandler.GetComponent<SpawnOnMouseClick>();

    }
    void Update()
    {
        UpdateTarget();     //sucht das Ziel
        if (target == null)   //führt nichts aus, wenn kein Ziel gefunden wurde
            return;

        // Turm dreht sich zum Ziel
        Vector3 direction = target.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        transform.rotation = Quaternion.Euler(0f, smoothedRotation.eulerAngles.y, 0f);

        // Wenn die Zeit zum Schießen gekommen ist, wird geschossen
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate; // Setze den Timer für den nächsten Schuss
        }

        fireCountdown -= Time.deltaTime;
    }

    void UpdateTarget()
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

    

    void Shoot()
    {
        // Erzeugt das Projektil an der Feuerposition und weist ihm die Richtung des Ziels zu
        if (!spawnScript.spawned)
        {
            damageScript = target.GetComponent<DamageTest>();
            damageScript.TakeDamage(damageAmount);

            StartCoroutine(Recoil() );  
        }
    }

    IEnumerator Recoil()
    {
        Vector3 originalPosition = canon.transform.localPosition;
        Vector3 recoilPosition = originalPosition - new Vector3(recoilDistance, 0, 0);

        float elapsedTime = 0;
        while(elapsedTime < recoilSpeed)
        {
            canon.transform.localPosition = Vector3.Lerp(originalPosition, recoilPosition,elapsedTime/recoilSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canon.transform.localPosition = recoilPosition;

        elapsedTime = 0;
        while(elapsedTime < recoilSpeed)
        {
            canon.transform.localPosition = Vector3.Lerp(recoilPosition, originalPosition, elapsedTime / recoilSpeed);
            elapsedTime += Time.deltaTime;  
            yield return null;
        }
        canon.transform.localPosition = originalPosition;
    }
    // **Upgrade-Funktion**
    public void UpgradeTower()
    {
        level++;
        damageAmount += 10;  // Erhöhe Schaden pro Level
        range += 2f;        // Erhöhe Reichweite pro Level
        fireRate += 0.2f;   // Schnellere Schussrate

        Debug.Log($"{gameObject.name} wurde auf Level {level} geupgradet!");
    }

    public int GetLevel()
    {
        return level;
    }
    // Zeichne den Turm-Radius im Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

