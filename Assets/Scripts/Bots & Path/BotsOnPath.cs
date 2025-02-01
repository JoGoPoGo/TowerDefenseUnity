using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[System.Serializable]
public class WaveConfiguration
{
    public int botsPerGroup = 3; // Anzahl der Bots pro Gruppe
    public float groupWaitTime = 3f; // Wartezeit zwischen Gruppen
    public float timeInGroup = 1f;  //Wartezeit zwischen einzelnen Bots in der Gruppe
    public GameObject[] botPrefabs; // Array mit unterschiedlichen Bot-Typen
    public float waveWaitTime = 10f; // Wartezeit zwischen Wellen
    public int groupsInWave = 3;
}
public class GroupConfiguration
{
    //Hier muss noch weiter gemacht werden
}
public class BotsOnPath : MonoBehaviour
{
    public WaveConfiguration[] waves; // wie viele Wellen?
    public PathCreator pathCreator;
    public float speedMultiplier = 2;
    
    public bool loopPath = false;

    //private int currentWave = 0;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    // Spawning der Wellen
    IEnumerator SpawnWaves()
    {
        for (int i = 0; i < waves.Length; i++) // Schleife durch alle Wellen
        {
            WaveConfiguration currentWave = waves[i]; // Aktuelle Welle abrufen
            //Debug.Log("Welle " + (i + 1) + " startet!");
            if (i == waves.Length - 1)
            {
                yield return StartCoroutine(SpawnGroupsInWave(currentWave, true)); // Spawne Gruppen in der aktuellen letzten Welle
                Debug.Log("Letzte Welle");
            }
            else
            {
                yield return StartCoroutine(SpawnGroupsInWave(currentWave, false));
                
            }
            Debug.Log("Welle " + (i + 1) + " beendet!");
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
            //yield return new WaitForSeconds(currentWave.waveWaitTime); // Wartezeit vor der nächsten Welle
        }

        Debug.Log("Alle Wellen abgeschlossen!");
    }

    // Spawning von Gruppen in einer Welle
    IEnumerator SpawnGroupsInWave(WaveConfiguration waveConfig, bool isLast)
    {
        for (int i = 0; i < waveConfig.groupsInWave; i++) // Anzahl der Gruppen in dieser Welle
        {
            if (isLast)
            {
                yield return StartCoroutine(SpawnGroup(waveConfig, true)); // Spawne eine Gruppe
                Debug.Log("Letzte Gruppe");
            }
            else
            {
                yield return StartCoroutine(SpawnGroup(waveConfig, false));
            }
            yield return new WaitForSeconds(waveConfig.groupWaitTime); // Wartezeit zwischen Gruppen
        }
    }

    // Spawnt eine Gruppe von Bots
    IEnumerator SpawnGroup(WaveConfiguration waveConfig, bool isLast)
    {
        for (int i = 0; i < waveConfig.botsPerGroup; i++) // Anzahl der Bots pro Gruppe
        {
            GameObject botPrefab = waveConfig.botPrefabs[Random.Range(0, waveConfig.botPrefabs.Length)]; // Zufälliger Bot
            if(isLast)
            {
                SpawnNewBot(botPrefab, true); // Bot spawnen
                Debug.Log("Letzter Bot");
            }
            else
            {
                SpawnNewBot(botPrefab, false);  
            }
            
            yield return new WaitForSeconds(waveConfig.timeInGroup); // Kurze Pause zwischen Bots (optional)
        }
    }


    // Spawnt einen neuen Bot
    void SpawnNewBot(GameObject botPrefab, bool isLast)
    {
        Vector3 spawnPosition = pathCreator.path.GetPoint(0);
        GameObject bot = Instantiate(botPrefab, spawnPosition, Quaternion.identity);
        DamageTest damageScript = bot.GetComponent<DamageTest>();
        if (isLast)
        {
            damageScript.isLast = true;
            //Debug.Log("letzten Bot gefunden");
        }
        else
        {
            damageScript.isLast = false;
        }
        StartCoroutine(MoveBotAlongPath(bot));
    }

    // Bewegung des Bots entlang des Pfads
    IEnumerator MoveBotAlongPath(GameObject enemy)
    {
        float distanceTravelled = 0f;
        DamageTest damageScript = enemy.GetComponent<DamageTest>();

        while (damageScript != null && damageScript.isAlive)
        {
            distanceTravelled += damageScript.speed * speedMultiplier * Time.deltaTime;
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
