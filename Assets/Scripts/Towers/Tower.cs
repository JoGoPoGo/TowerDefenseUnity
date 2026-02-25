
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class Tower : MonoBehaviour
{
    [Header("Stats")]
    public float range = 15f;          // Reichweite des Turms
    public float rangeMinimum = 0f;

    public int damageAmount = 50;       //Schaden
    public float fireRate = 1f;        // Schussfrequenz

    public int level = 1;  // Turm-Level beginnt bei 1
    public int maxLvl = 10;
    public int upgradeCost = 1;

    [Header("Upgrade Variables")]
    //public float upgradePercentage = 50;
    public int rangeBool = 0;
    public int damageBool = 0;
    public int fireRateBool = 0;
    public int cancelBool = 0;

    [Header("Changeables")]
    public int price;
    public int sellReturn;
    public int spawnCancelRadius = 10;         // Kein Weiterer turm in diesem Bereich
    public float turnSpeed = 10f;      // Geschwindigkeit, mit der der Turm sich dreht
    public float recoilSpeed = 0.1f;
    public float recoilDistance = 0.2f;
    public GameObject canon;
    public string enemyTag = "Enemy";  // Der Tag der Gegner (z.B. "Enemy")
    public int maxHigher = 0;

    [Header("Scripts")]
    public SpawnOnMouseClick spawnScript; // Reference to the SpawnOnMouseClick script
    protected CancelDictionaryProtoType dictionary;
    protected EnemyScript damageScript;   // DamageTest von Target
    private GameManager gameManager;

    [Header("Audio")]
    public AudioClip shootSound;
    protected AudioSource audioSource;

    [Header("Funktion")]
    public GameObject target;           // Das aktuelle Ziel des Turms
    public ParticleSystem disturbEffect;
    public Animator animator;

    public GameObject projectilePrefab;
    public GameObject firePoint;

    protected float fireCountdown = 0f; 
    private Tower[] allTowerComponents;   
    protected float UpdateCounter = 0;
    protected bool dictionaryActivater = true;
    protected LayerMask obstacleMask;
    protected int tiling = 1;
    protected float updateTargetIntervall = 0.5f;

    public float startSubtract = 0f;
 

    protected virtual void Start()
    {
        obstacleMask = LayerMask.GetMask("Obstacle");
        GameObject spawnHandler = GameObject.Find("SpawnHandler");

        spawnScript = spawnHandler.GetComponent<SpawnOnMouseClick>();
        tiling = spawnScript.tiling;

        allTowerComponents = GetComponents<Tower>();      //Liste aller Komponenten der Towerklasse
        gameManager = FindObjectOfType<GameManager>();

        dictionary = gameManager.GetComponent<CancelDictionaryProtoType>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();

            // Optional: Wenn der Sound leiser werden soll, je weiter die Kamera weg ist
            audioSource.spatialBlend = 1f; 
        }
    }
    protected virtual void Update()  
    {
        if (gameObject.CompareTag("Tower") && dictionaryActivater)  //wenn der Turm das erste Mal "Tower" ist
        {
            Vector2Int posi = new Vector2Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));

            OccupyPositionsInCircle(posi, spawnCancelRadius);   //werden innerhalb des SpawnCancelRadius' keine Türme mehr spawnbar gemacht
            dictionaryActivater = false;
        }

        if(UpdateCounter >= updateTargetIntervall)  //UpdateTarget soll in Abständen von updateTargetIntervall ausgeführt werden
            UpdateCounter = 0;
        if(UpdateCounter == 0 && gameObject.CompareTag("Tower"))    //wenn der Turm "Tower" ist
            UpdateTarget();  
        UpdateCounter += Time.deltaTime;

        if (target == null)   //führt nichts aus, wenn kein Ziel gefunden wurde
            return;
        
        
        RotateTo(target);   // Turm dreht sich zum Ziel

        /// ---------FireRate--------------"Wenn die Zeit zum Schießen gekommen ist, wird geschossen"
        if (fireCountdown <= 0f)
        {
            if (gameObject.CompareTag("Tower"))
            {
                Shoot();
            }

            fireCountdown = 1f / fireRate; // Setze den Timer für den nächsten Schuss
        }
        
        fireCountdown -= Time.deltaTime;
    }
    protected virtual void RotateTo(GameObject t)   //Rotiert den Turm zum GameObject t
    {
        Vector3 direction = t.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        transform.rotation = Quaternion.Euler(0f, smoothedRotation.eulerAngles.y, 0f);
    }

    public string getName()
    {
        return(name);
    }
    protected virtual void UpdateTarget()   // protected für Schussfunktionen und Animationen
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        float shortestDistance = range; // maximale Reichweite
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)   //prüft jeden Gegner in der Szene
        {
            Vector3 direction = (enemy.transform.position + new Vector3(0,2f,0)) - transform.position;
            float distance = direction.magnitude;

            // Mindest- UND Maximalreichweite UND höhenunterschied prüfen
            if (distance >= rangeMinimum && distance <= shortestDistance && enemy.transform.position.y <= (transform.position.y + 2f + (float)maxHigher))
            {
                // Sichtprüfung (keine Hindernisse dazwischen)
                if (!Physics.Raycast(transform.position + new Vector3(0,2f,0), direction.normalized, distance, obstacleMask))
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }

        target = nearestEnemy; // bleibt null, wenn kein gültiges Ziel gefunden wurde
    }

    protected virtual void Shoot()         // protected für Schussanimationen und Funktionen
    {
        if (!spawnScript.spawned)
        {
            // Sound abspielen!
            if (shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
            if (projectilePrefab != null)    //Wenn es ein Projektil gibt,...
            {
                GameObject pro = Instantiate(
                    projectilePrefab,
                    firePoint.transform.position,
                    firePoint.transform.rotation
                );

                pro.transform.SetParent(firePoint.transform, true);
                StartCoroutine(ProjectileAnimation(pro, target));   //...wird es hiermit (ProjectileAnimation) bewegt
            }
            else               //Wenn nicht, wird direkt getroffen Ausgeführt
            {
                damageScript = target.GetComponent<EnemyScript>();
                hitEnemy(damageScript);
            }

            /*if (canon != null)
            {
                Debug.Log("bola");
            }*/
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
        if(level < maxLvl)
        {
            if (gameManager.SpendCredits((upgradeCost)))
            {
                level++;
                if (rangeBool > 0)
                {
                    range *= (1 + (float)rangeBool/100);
                    Debug.Log("Range Upgradet auf: " + range);
                }

                if (damageBool > 0)
                {
                    float save = damageAmount * (1  + (float)damageBool/100);
                    damageAmount = (int)Mathf.Round(save);
                }
                if (fireRateBool > 0)
                {
                    fireRate *= (1 + (float)fireRateBool/100);
                }

                if (cancelBool > 0)
                {
                    float save = spawnCancelRadius * (1 + (float)cancelBool / 100);
                    spawnCancelRadius = (int)Mathf.Round(save);
                }

                sellReturn += (int)Mathf.Round(upgradeCost / 2);
                upgradeCost = (int)Mathf.Round((float)upgradeCost * 1.2f) + 1;  //steigert den UpgradePreis um 20%

                Debug.Log($"{gameObject.name} wurde auf Level {level} geupgradet!");
            }
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
    protected virtual IEnumerator ProjectileAnimation(GameObject projectile, GameObject enemy)  //Wurfartige Flugbahn von projectile zum Gegner  
    {
        float distance = Vector3.Distance(projectile.transform.position, enemy.transform.position);
        float duration = 0.4f;
        float t = 0f;
        Vector3 startPos = projectile.transform.position;
        Vector3 targetPos = enemy.transform.position + new Vector3(0, 1f, 0);
        float yPosTarget = targetPos.y;
        float yPosCurrent = startPos.y;
        targetPos.y = startPos.y;

        float subtract = startSubtract; //Wenn startSubtract (siehe Inspector) kleiner null ist, zeigt der Startwinkel nach Oben

        while (t < duration)    //Flugbahn
        {
            yPosCurrent -= subtract;
            subtract += (startPos.y - yPosTarget) / Sum0ToN((int)Mathf.Round(duration / Time.deltaTime));
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t / duration);
            currentPos.y = yPosCurrent;
            projectile.transform.position = currentPos;
            t += Time.deltaTime;
            yield return null;
        }

        if (enemy != null)
            damageScript = enemy.GetComponent<EnemyScript>();
        if (damageScript != null)
            hitEnemy(damageScript);

        Destroy(projectile);
    }

    int Sum0ToN(int n)
    {
        return n * (n + 1) / 2;
    }

    //Sell wird beim Verkaufen ausgeführt
    virtual public void Sell()
    {
        gameManager.AddCredits(sellReturn);  //"Zurückzahlen"

        TowerInfoUI infoUIScript = gameManager.GetComponent<TowerInfoUI>();             //Verstecken der RangePreview & des TowerInfo - Panels

        DynamicRangePreview rangeScript = gameObject.GetComponent<DynamicRangePreview>();       
        WorldPreview worldPreviewScript = rangeScript.previewScript;

        infoUIScript.Hide();
        worldPreviewScript.Hide();

        Destroy(gameObject);
    }

    public void PlayDisturb()
    {
        if (animator != null)
            animator.SetBool("Disturbed", true);

        if (disturbEffect != null && !disturbEffect.isPlaying)
            disturbEffect.Play();
    }

    public void StopDisturb()
    {
        if (animator != null)
            animator.SetBool("Disturbed", false);

        if (disturbEffect != null)
            disturbEffect.Stop();
    }

    protected virtual void hitEnemy(EnemyScript damageTest)  //wird beim Treffen des Gegners ausgeführt
    {
        damageScript.TakeDamage(damageAmount);
    }
}

