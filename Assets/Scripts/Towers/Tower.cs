using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public SpawnOnMouseClick spawnScript; // Reference to the SpawnOnMouseClick script
    public Transform target;           // Das aktuelle Ziel des Turms
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

    void Start()
    {
        GameObject spawnHandler = GameObject.Find("SpawnHandler");
        spawnScript = spawnHandler.GetComponent<SpawnOnMouseClick>();

        // Sucht alle paar Sekunden nach Gegnern
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
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
            target = nearestEnemy.transform;
        }
        else
        {
            target = null; // Kein Gegner in Reichweite
        }
    }

    void Update()
    {
        if (target == null)
            return;

        // Turm dreht sich langsam zum Ziel
        Vector3 direction = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        // Wenn die Zeit zum Schießen gekommen ist, wird geschossen
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate; // Setze den Timer für den nächsten Schuss
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        // Erzeugt das Projektil an der Feuerposition und weist ihm die Richtung des Ziels zu
        if (!spawnScript.spawned)
        {
            GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }

    // Zeichne den Turm-Radius im Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

