using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[System.Serializable]
public class WaveConfiguration
{
    public GroupConfiguration[] groups;  //alle Gruppen in dieser Welle
    public int waitAfterWaveEnd = 0;
}
[System.Serializable]
public class GroupConfiguration
{
    public List<BotConfiguration> botConfigs;
    public float tillNextGroup;
    public float tillNextBot;        //Wartezeit zwischen den Bots

}
[System.Serializable]
public class BotConfiguration
{
    public GameObject botPrefab;     //Welcher Bot gespawnt wird
    public int timesBot;             //Wie oft der Bot gespawnt wird
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
            yield return new WaitForSeconds(0.1f);   //damit jede Aktuelle welle gespawned wird
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
            yield return new WaitForSeconds(currentWave.waitAfterWaveEnd);
            //yield return new WaitForSeconds(currentWave.waveWaitTime); // Wartezeit vor der nächsten Welle
        }

        Debug.Log("Alle Wellen abgeschlossen!");
    }

    // Spawning von Gruppen in einer Welle
    IEnumerator SpawnGroupsInWave(WaveConfiguration waveConfig, bool isLast)
    {
        for (int i = 0; i < waveConfig.groups.Length; i++) // Anzahl der Gruppen in dieser Welle
        {
            GroupConfiguration currentGroup = waveConfig.groups[i];

            if (isLast)
            {
                yield return StartCoroutine(SpawnGroup(currentGroup, true)); // Spawne eine Gruppe
                Debug.Log("Letzte Gruppe");
            }
            else
            {
                yield return StartCoroutine(SpawnGroup(currentGroup, false));
            }

            // Wartezeit nach der Gruppe, bevor die nächste Gruppe startet
            yield return new WaitForSeconds(currentGroup.tillNextGroup);
        }
    }

    // Spawnt eine Gruppe von Bots
    IEnumerator SpawnGroup(GroupConfiguration groupConfig, bool isLast)
    {
        foreach (BotConfiguration botConfig in groupConfig.botConfigs)
        {
            for (int i = 0; i < botConfig.timesBot; i++) // Anzahl der Bots pro Gruppe
            {
                
                SpawnNewBot(botConfig.botPrefab, isLast);
                

                yield return new WaitForSeconds(groupConfig.tillNextBot); // Kurze Pause zischen Bots (optional)
            }
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

        // Überträgt variablen
        damageScript.pathCreator = pathCreator;
        damageScript.speedMultiplier = speedMultiplier;
        damageScript.thisBotScript = this;
        //StartCoroutine(MoveBotAlongPath(bot));
    }

    // Bewegung des Bots entlang des Pfads
    IEnumerator MoveBotAlongPath(GameObject enemy)
    {
        float distanceTravelled = 0f;
        DamageTest damageScript = enemy.GetComponent<DamageTest>();
        yield return null;   // randomized wird erst genommen, wenn DamageTest einen Wert != 0 dafür hat
        Vector3 randomized = damageScript.positionRandomizer;

        while (damageScript != null && damageScript.isAlive)
        {
            distanceTravelled += damageScript.speed * speedMultiplier * Time.deltaTime;
            enemy.transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled) + randomized;
            enemy.transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled) * Quaternion.Euler(0, 0, 90);

            if (distanceTravelled >= pathCreator.path.length)    // Zerstört den Gegner, wenn er das Ende des Pfades erreicht
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
