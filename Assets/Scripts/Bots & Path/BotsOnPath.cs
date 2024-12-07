using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class BotsOnPath : MonoBehaviour
{
    public GameObject[] botPrefabs; // Array mit unterschiedlichen Bot-Typen
    public PathCreator pathCreator;
    public float moveSpeed = 5f;
    public float groupWaitTime = 3f; // Wartezeit zwischen Gruppen
    public float waveWaitTime = 10f; // Wartezeit zwischen Wellen
    public float timeInGroup = 1f;  //Wartezeit zwischen einzelnen Bots in der Gruppe
    public int totalWaves = 5; // Anzahl der Wellen
    public int botsPerGroup = 3; // Anzahl der Bots pro Gruppe
    public bool loopPath = false;

    private int currentWave = 0;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    // Spawning der Wellen
    IEnumerator SpawnWaves()
    {
        while (currentWave < totalWaves)
        {
            Debug.Log("Welle " + (currentWave + 1) + " startet!");
            yield return StartCoroutine(SpawnGroupsInWave());
            currentWave++;
            Debug.Log("Welle " + currentWave + " beendet!");
            yield return new WaitForSeconds(waveWaitTime);
        }

        Debug.Log("Alle Wellen abgeschlossen!");
    }

    // Spawning von Gruppen in einer Welle
    IEnumerator SpawnGroupsInWave()
    {
        int totalGroups = currentWave + 1; // Jede Welle hat eine Gruppe mehr als die letzte
        for (int i = 0; i < totalGroups; i++)
        {
            // Startet das Spawnen einer Gruppe
            yield return StartCoroutine(SpawnGroup());

            // Wartezeit zwischen den Gruppen
            yield return new WaitForSeconds(groupWaitTime);
        }
    }

    // Spawnt eine Gruppe von Bots
    IEnumerator SpawnGroup()
    {
        for (int i = 0; i < botsPerGroup; i++)
        {
            // Wählt einen zufälligen Bot aus der Liste
            GameObject botPrefab = botPrefabs[Random.Range(0, botPrefabs.Length)];

            // Spawnt den Bot
            SpawnNewBot(botPrefab);

            // Wartezeit zwischen den einzelnen Bots in der Gruppe
            yield return new WaitForSeconds(timeInGroup);
        }
    }


    // Spawnt einen neuen Bot
    void SpawnNewBot(GameObject botPrefab)
    {
        Vector3 spawnPosition = pathCreator.path.GetPoint(0);
        GameObject bot = Instantiate(botPrefab, spawnPosition, Quaternion.identity);
        StartCoroutine(MoveBotAlongPath(bot));
    }

    // Bewegung des Bots entlang des Pfads
    IEnumerator MoveBotAlongPath(GameObject enemy)
    {
        float distanceTravelled = 0f;
        DamageTest damageScript = enemy.GetComponent<DamageTest>();

        while (damageScript != null && damageScript.isAlive)
        {
            distanceTravelled += moveSpeed * Time.deltaTime;
            enemy.transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
            enemy.transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled) * Quaternion.Euler(0, 0, 90);

            if (distanceTravelled >= pathCreator.path.length)
            {
                if (loopPath)
                {
                    distanceTravelled = 0f;
                }
                else
                {
                    Destroy(enemy);
                    yield break;
                }
            }

            yield return null;
        }

        Destroy(enemy); // Bot wird zerstört, wenn er nicht mehr "alive" ist
    }
}


