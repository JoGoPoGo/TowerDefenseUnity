using PathCreation;
using System;
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

    public PathCreator pathCreator;
    private BaseHealth baseScript;
    public float speedMultiplier;
    public bool isAlive = true;
    public Animator animator;

    [Header("Funktion")]
    public bool isLast = false;
    public bool isDebuffed = false;

    public Vector3 positionRandomizer;
    public float distanceTravelled = 0f;

    private bool isDead = false;

    protected virtual void Start()
    {
        baseScript = FindObjectOfType<BaseHealth>();
        // Setze die Lebenspunkte auf das Maximum
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth); // Update der Lebensanzeige
        gameManager = FindObjectOfType<GameManager>();
        positionRandomizer = new Vector3 (UnityEngine.Random.Range(-randomization,randomization), 0, UnityEngine.Random.Range(-randomization,randomization));
    }
    protected virtual void Update()
    {
        if (pathCreator != null)
        {
            MoveAlongPath(pathCreator, distanceTravelled);
        }
        distanceTravelled += speed * speedMultiplier * Time.deltaTime;
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
                if (thisBotScript.deadBotsInLastWave >= thisBotScript.totalBotsInLastWave)
                {
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

    public void MoveAlongPath(PathCreator path, float pathPoint)    // soll als Ersatz für IEnumarator bei BotsOnPath dienen, welche die Bots bewegt
    {
        transform.position = path.path.GetPointAtDistance(pathPoint) + positionRandomizer;
        transform.rotation = path.path.GetRotationAtDistance(pathPoint) * Quaternion.Euler(0, 0, 90);
        if(distanceTravelled >= path.path.length)
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
}
