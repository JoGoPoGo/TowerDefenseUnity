using PathCreation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class DamageTest : MonoBehaviour
{
    public bool isAlive = true;

    public int currentHealth; // Aktuelle Leben -- siehe public
    public int maxHealth = 100; // Maximale Leben -- siehe public
    public float speed = 1f;
    public int reward = 0;

    public HealthSlider healthbar; // Referenz zur Lebensanzeige  -- siehe public

    public bool isLast;

    protected GameManager gameManager;

    public float randomization;
    public Vector3 positionRandomizer;

    //von BotsOnPath
    public PathCreator pathCreator;
    public float speedMultiplier;

    public float distanceTravelled = 0f;

    protected virtual void Start()
    {
        // Setze die Lebenspunkte auf das Maximum
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth); // Update der Lebensanzeige
        gameManager = FindObjectOfType<GameManager>();
        positionRandomizer = new Vector3 (UnityEngine.Random.Range(-randomization,randomization), 0, UnityEngine.Random.Range(-randomization,randomization));
    }
    protected void Update()
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
        if (!didDamage)
        {
            gameManager.AddCredits(reward);
        }
        if (IsOnlyEnemy() && isLast)
        {
            BotsOnPath[] botsOnPaths = FindObjectsOfType<BotsOnPath>();

            if(botsOnPaths.Length == 1)
            {
                Debug.Log("Is only enemy and the last one at DamageTest.79");
                //SceneManager.LoadScene("LevelAuswahl");   //Auskommentieren, falls es zu unerwünschten Szenenwechsel kommt
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
            else
            {

            }
            
        }
        isAlive = false;
        Destroy(gameObject);
    }
    public void MoveAlongPath(PathCreator path, float pathPoint)    // soll als ersatz für IEnumarator bei BotsOnPath dienen, welche die Bots bewegt
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
}
