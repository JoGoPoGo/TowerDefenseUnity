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

    private GameManager gameManager;

    public float randomization;
    public Vector3 positionRandomizer;

    //von BotsOnPath
    public PathCreator pathCreator;
    public float speedMultiplier;

    float distanceTravelled = 0f;

    void Start()
    {
        // Setze die Lebenspunkte auf das Maximum
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth); // Update der Lebensanzeige
        gameManager = FindObjectOfType<GameManager>();
        positionRandomizer = new Vector3 (UnityEngine.Random.Range(-randomization,randomization), 0, UnityEngine.Random.Range(-randomization,randomization));
    }
    private void Update()
    {
        if (pathCreator != null)
        {
            MoveAlongPath(pathCreator, distanceTravelled);
        }
        distanceTravelled = speed * speedMultiplier * Time.deltaTime;
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
    public void Die(bool didDamage)
    {
        if (!didDamage)
        {
            gameManager.AddCredits(reward);
        }
        if (IsOnlyEnemy() && isLast)
        {
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
        isAlive = false;
        Destroy(gameObject);
    }
    public void MoveAlongPath(PathCreator path, float pathPoint)    // soll als ersatz für IEnumarator bei BotsOnPath dienen, welche die Bots bewegt
    {
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled) + positionRandomizer;
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled) * Quaternion.Euler(0, 0, 90);
        if(distanceTravelled >= pathCreator.path.length)
        {
            Destroy(gameObject);
        }
    }
    public bool IsOnlyEnemy()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length == 1; //true, wenn es keine weiteren Gegner gibt.
    }
}
