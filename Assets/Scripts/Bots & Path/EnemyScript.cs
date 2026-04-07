using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyScript : MonoBehaviour
{
    [Header("Stats")]
    public int currentHealth; // Aktuelle Leben -- siehe public
    public int maxHealth = 100; // Maximale Leben -- siehe public
    public float speed = 1f;
    public int reward = 0;
    public float randomization;

    [Header("Referenz")]
    public HealthSlider healthbar; // Referenz zur Lebensanzeige  -- siehe public
    protected GameManager gameManager;
    public BotsOnPath thisBotScript; //von BotsOnPath
    public BotsOnPath[] otherBotScripts; //wird in BotsOnPath zugewiesen

    public PathCreator pathCreator;
    private BaseHealth baseScript;
    public float speedMultiplier;
    public bool isAlive = true;
    public Animator animator;
    public ParticleSystem shieldAktivParticles;

    [Header("Funktion")]
    public bool isLast = false;
    public int wave;
    public bool isDebuffed = false;

    public bool shieldAktiv = false;
    public float shieldAktivSeconds;

    public float hoverAmplitude = 1f;   // Höhe der Bewegung
    public float hoverFrequency = 2f;   // Geschwindigkeit der Welle

    private bool aktivate = true;

    public Vector3 positionRandomizer;
    public float distanceTravelled = 0f;

    public bool chake = false;

    private bool isDead = false;

    private Terrain terrain;

    [Header("Movement Variation")]
    public float corridorWidth = 10f;       // Wie weit links/rechts vom Pfad
    public float driftSpeed = 0.6f;          // Wie schnell der Gegner seitlich driftet
    public float driftAmount = 0.25f;         // Wie stark die Drift ist
    public float rotationSmoothness = 7f;    // Wie weich der Gegner dreht
    public float lookAheadDistance = 0.8f;   // Wie weit nach vorne für Rotation geschaut wird

    private float baseLateralOffset;         // Fester seitlicher Offset pro Gegner
    private float driftSeed;                 // Zufälliger Seed für individuelle Drift

    protected virtual void Start()
    {
        hoverAmplitude = Random.Range(hoverAmplitude * 0.9f, hoverAmplitude * 1.1f);
        hoverFrequency = Random.Range(hoverFrequency * 0.9f, hoverFrequency * 1.1f);

        for (int i = 1; i < wave; i++)
        {
            maxHealth = (int)Mathf.Round(maxHealth * 1.2f);
        }
        baseScript = FindObjectOfType<BaseHealth>();
        // Setze die Lebenspunkte auf das Maximum
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth); // Update der Lebensanzeige
        gameManager = FindObjectOfType<GameManager>();
        baseLateralOffset = Random.Range(-corridorWidth, corridorWidth);
        driftSeed = Random.Range(0f, 100f);

        // Optional kleine Geschwindigkeitsvariation
        speed *= Random.Range(0.95f, 1.05f);

        // Optional leichte Animationsvariation
        if (animator != null)
        {
            animator.speed = Random.Range(0.97f, 1.03f);
        }

        if (aktivate)
        {
            StartCoroutine(AktivateShield(shieldAktivSeconds));
        }
        terrain = Terrain.activeTerrain;
        
    }
    protected virtual void Update()
    {
        if (pathCreator != null)
        {
            MoveAlongPath(pathCreator, distanceTravelled);
        }
        distanceTravelled += speed * speedMultiplier * Time.deltaTime;
        if(shieldAktiv && aktivate)
        {
            StartCoroutine(AktivateShield(shieldAktivSeconds));
        }
        if(currentHealth > maxHealth) 
        {
            maxHealth = currentHealth;
            healthbar.SetMaxHealth(maxHealth);
        }
    }

    // Funktion zum Zufügen von Schaden
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Schaden anwenden

        healthbar.Sethealth(currentHealth); // Lebensanzeige aktualisieren

        // Wenn das Leben auf 0 oder darunter fällt, zerstöre den Bot
        if (currentHealth <= 0)
        {
            Die(false);
        }
    }

    // Zerstört den Bot, wenn die Lebenspunkte auf 0 fallen
    public virtual void Die(bool didDamage)
    {
        if (!isDead)
        {
            isDead = true;
            if (isLast)
            {
                thisBotScript.deadBotsInLastWave++;

                // Prüfen, ob ALLE Bots gespawnt und ALLE tot sind
                bool allFinished = true;

                foreach (BotsOnPath bot in otherBotScripts) //prüft für alle BotsOnPath Skripte
                {
                    Debug.Log(otherBotScripts);
                    if (bot.deadBotsInLastWave < bot.totalBotsInLastWave)
                    {
                        allFinished = false;
                        break;
                    }
                }

                if (allFinished)
                {
                    
                    Debug.Log("allFinished");
                    TriggerWin();
                }
            }
            if (!didDamage)
            {
                gameManager.AddCredits(reward);
            }
            if (animator != null)
            {
                animator.SetBool("isDead", true);
                speed = 0;
                gameObject.tag = "Untagged";      //tag und layer ändern, damit die Türme nicht mehr angreifen
                gameObject.layer = 0;
                healthbar.Sethealth(-1);
                StartCoroutine(WaitDeath(2));
            }
            else
            {
                Destroy(gameObject);
            }
        }
        

    }

    private IEnumerator WaitDeath(int a)
    {
        yield return new WaitForSeconds(a);
        Destroy(gameObject);
    }

    public void MoveAlongPath(PathCreator path, float pathPoint)
    {
        // 1. Mittelpunkt auf dem Pfad
        Vector3 centerPos = path.path.GetPointAtDistance(pathPoint);

        // 2. Richtung des Pfades an dieser Stelle
        Vector3 forward = path.path.GetDirectionAtDistance(pathPoint).normalized;

        // 3. Seitliche Richtung relativ zum Pfad
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

        // 4. Sanfte Drift innerhalb des Korridors
        float drift = Mathf.Sin(Time.time * driftSpeed + driftSeed) * driftAmount;

        // 5. Endgültiger seitlicher Offset
        float finalOffset = baseLateralOffset + drift;

        // Begrenzen, damit Gegner nicht zu weit vom Weg wegkommen
        finalOffset = Mathf.Clamp(finalOffset, -corridorWidth, corridorWidth);

        // 6. Position berechnen
        Vector3 pos = centerPos + right * finalOffset;
        pos.y = terrain.SampleHeight(pos);

        // 7. Optional Hover / Fliegen
        if (chake)
        {
            float sinOffset = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
            pos.y += sinOffset;
        }

        // 8. Position setzen
        transform.position = pos;

        // 9. Weiche Rotation mit Blick etwas nach vorne
        Vector3 futurePos = path.path.GetPointAtDistance(pathPoint + lookAheadDistance);
        Vector3 futureForward = path.path.GetDirectionAtDistance(pathPoint + lookAheadDistance).normalized;
        Vector3 futureRight = Vector3.Cross(Vector3.up, futureForward).normalized;

        float futureDrift = Mathf.Sin(Time.time * driftSpeed + driftSeed) * driftAmount;
        float futureOffset = Mathf.Clamp(baseLateralOffset + futureDrift, -corridorWidth, corridorWidth);

        futurePos += futureRight * futureOffset;

        Vector3 moveDir = (futurePos - transform.position).normalized;
        moveDir.y = 0;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir) * Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSmoothness * Time.deltaTime);
        }

        // 10. Base erreicht
        if (distanceTravelled >= path.path.length)
        {
            Die(true);
        }
    }
    public bool IsOnlyEnemy()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length == 1; //true, wenn es keine weiteren Gegner gibt.
    }
    public virtual void TriggerWin()
    {
        GameObject parent = GameObject.Find("Canvas");
        if (parent != null)
        {
            Transform winTransform = parent.transform.Find("WinScreen");
            if (winTransform != null)
            {
                winTransform.gameObject.SetActive(true);
            }
        }
    }
    public IEnumerator AktivateShield(float seconds)
    {   
        aktivate = false;
        this.tag = "EnemyNot";
        if (shieldAktivParticles != null)
        {
            shieldAktivParticles.gameObject.SetActive(true);
            shieldAktivParticles.Play();
        }

        yield return new WaitForSeconds(seconds);
        this.tag = "Enemy";
        if (shieldAktivParticles != null)
        {
            shieldAktivParticles.Stop();
            shieldAktivParticles.gameObject.SetActive(false);
            
        }

        aktivate = true;
    }
}
