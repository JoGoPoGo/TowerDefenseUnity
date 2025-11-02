using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class Tower : MonoBehaviour
{
    public SpawnOnMouseClick spawnScript; // Reference to the SpawnOnMouseClick script
    public GameObject target;           // Das aktuelle Ziel des Turms
    protected DamageTest damageScript;   // DamageTest von Target
    private GameManager gameManager;

    public string enemyTag = "Enemy";  // Der Tag der Gegner (z.B. "Enemy")

    public int price;
    public float fireRate = 1f;        // Schussfrequenz
    public float bulletSpeed = 10f;     //Kugelgeschwindigkeit
    public float range = 15f;          // Reichweite des Turms
    public int damageAmount = 50;       //Schaden
    public int spawnCancelRadius = 10;        // Kein Weiterer turm in diesem Bereich
   
    protected float fireCountdown = 0f; 
    
   
    public float turnSpeed = 10f;      // Geschwindigkeit, mit der der Turm sich dreht

    public GameObject canon;

    public float recoilSpeed = 0.1f;
    public float recoilDistance = 0.2f;

    public int level = 1;  // Turm-Level beginnt bei 1

    public AudioSource shootSound;

    private Tower[] allTowerComponents;

    private int UpdateCounter = 0;
    protected
    bool dictionaryActivater = true;

    //public string name;

    protected LayerMask obstacleMask;

    protected CancelDictionaryProtoType dictionary;

    protected int tiling = 1;

    protected virtual void Start()
    {
        obstacleMask = LayerMask.GetMask("Obstacle");
        GameObject spawnHandler = GameObject.Find("SpawnHandler");

        spawnScript = spawnHandler.GetComponent<SpawnOnMouseClick>();
        tiling = spawnScript.tiling;

        allTowerComponents = GetComponents<Tower>();      //Liste aller Komponenten der Towerklasse
        gameManager = FindObjectOfType<GameManager>();

        dictionary = gameManager.GetComponent<CancelDictionaryProtoType>();



    }
    protected virtual void Update()  
    {
        if (gameObject.CompareTag("Tower") && dictionaryActivater)
        {
            Vector2Int posi = new Vector2Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));

            OccupyPositionsInCircle(posi, spawnCancelRadius);
            dictionaryActivater = false;
        }

        if(UpdateCounter == 10)
        {
            UpdateCounter = 0;
        }

        if(UpdateCounter == 0 && gameObject.CompareTag("Tower"))
        {
            UpdateTarget();  
        }

        UpdateCounter ++;

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
            if (gameObject.CompareTag("Tower"))
            {
                shootSound.Play();
                Shoot();
            }

            fireCountdown = 1f / fireRate; // Setze den Timer für den nächsten Schuss
        }
        
        fireCountdown -= Time.deltaTime;
    }
    public string getName()
    {
        return(name);
    }
    protected virtual void UpdateTarget()   // protected für Schussfunktionen und Animationen
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = range; // Nur Gegner **innerhalb der Reichweite** sind relevant
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            Vector3 direction = enemy.transform.position - transform.position;
            float distance = direction.magnitude;

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

    

    protected virtual void Shoot()         // protected für Schussanimationen und Funktionen
    {
        // Erzeugt das Projektil an der Feuerposition und weist ihm die Richtung des Ziels zu#
        if (!spawnScript.spawned)
        {
            damageScript = target.GetComponent<DamageTest>();
            damageScript.TakeDamage(damageAmount);
            if (canon != null)
            {
                Debug.Log("bola");
            }
            if (allTowerComponents.Length <= 1 && canon != null)
            {
                StartCoroutine(Recoil());
            }
        }
    }

    IEnumerator Recoil()
    {
        Debug.Log("Recoil wird ausgeführt");
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
    virtual public void UpgradeTower()
    {
        if (gameManager.SpendCredits((level * level + 2)))
        {
            level++;
            damageAmount += 10;  // Erhöhe Schaden pro Level
            range += 2f;        // Erhöhe Reichweite pro Level
            //fireRate += 0.2f;   // Schnellere Schussrate

            Debug.Log($"{gameObject.name} wurde auf Level {level} geupgradet!");
        }
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
    public void OccupyPositionsInCircle(Vector2Int center, int range)
    {
        int rangeSqr = range * range;

        for(int x = center.x -range; x <= center.x + range; x += tiling)
        {
            for (int y = center.y - range; y <= center.y + range; y += tiling)
            {
                int dx = x - center.x;
                int dy = y - center.y;

                if (dx * dx + dy * dy <= rangeSqr)
                {
                    dictionary.OccupyPosition(new Vector2Int(x,y));

                }

            }
        }
    }
}

