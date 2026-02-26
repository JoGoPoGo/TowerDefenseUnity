using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class WaveConfiguration
{
    public GroupConfiguration[] groups;  //alle Gruppen in dieser Welle
    public int waitBevorWaveStart = 0;
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
    private BotsOnPath[] otherBotsOnPath;

    public float speedMultiplier = 2;
    
    public bool loopPath = false;

    public int totalBotsInLastWave = 0;
    public int deadBotsInLastWave = 0;

    private int totalBots = 0;

    public GameObject CountdownObject;
    private TextMeshProUGUI countdownText;

    private Button ButtonSkip;
    public GameObject Skip;
    private bool SkipCountdown = false;

    //private int currentWave = 0;

    void Start()
    {
        countdownText = CountdownObject.GetComponent<TextMeshProUGUI>();
        CountdownObject.SetActive(false);
        CountBotsInLastWave();
        StartCoroutine(SpawnWaves());

        ButtonSkip = Skip.GetComponent<Button>();
        ButtonSkip.onClick.AddListener(OnSkip);

        otherBotsOnPath = FindObjectsOfType<BotsOnPath>();
    }

    private void Update()
    {
        if(deadBotsInLastWave > totalBots)
        {
            Debug.Log("Es wurden bereits " + deadBotsInLastWave + " von " + totalBotsInLastWave + " getötet");
            totalBots = deadBotsInLastWave;
        }
    }

    void CountBotsInLastWave()
    {
        WaveConfiguration lastWave = waves[waves.Length - 1];

        foreach (var group in lastWave.groups)
        {
            foreach (var bot in group.botConfigs)
            {
                totalBotsInLastWave += bot.timesBot;
            }
        }
    }

    // Spawning der Wellen
    IEnumerator SpawnWaves()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            WaveConfiguration currentWave = waves[i];
            yield return new WaitForSeconds(0.1f);

            bool isLastWave = (i == waves.Length - 1);

            //CountDown bis zum Start der Welle
            Skip.SetActive(true);
            CountdownObject.SetActive(true);


            for (int k = currentWave.waitBevorWaveStart; k > 0; k--)
            {
                if (SkipCountdown)
                    break;
                if (countdownText != null)
                {
                    if (i != waves.Length - 1) //wenn es nicht die letzte Welle ist
                        countdownText.text = "Welle " + (i + 1) + " beginnt in " + k + " Sekunden";
                    if (i == waves.Length - 1) // wenn es die letzte Welle ist
                        countdownText.text = "Letzte Welle beginnt in " + k + " Sekunden";
                    //Debug.Log("Welle " + (i + 1) + " in " + k + " Sekunden");
                }
                yield return new WaitForSeconds(1);               
            }
            SkipCountdown = false;
            CountdownObject.SetActive(false);
            Skip.SetActive(false);
            // Ende des Countdowns

            yield return StartCoroutine(SpawnGroupsInWave(currentWave, isLastWave));

            Debug.Log("Welle " + (i + 1) + " beendet!");

            // Warte bis alle Gegner dieser Welle TOT sind
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

            
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
        EnemyScript damageScript = bot.GetComponent<EnemyScript>();
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
        damageScript.otherBotScripts = otherBotsOnPath;
        //StartCoroutine(MoveBotAlongPath(bot));
    }

    // Bewegung des Bots entlang des Pfads
    IEnumerator MoveBotAlongPath(GameObject enemy)
    {
        float distanceTravelled = 0f;
        EnemyScript damageScript = enemy.GetComponent<EnemyScript>();
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
    void OnSkip()
    {
        SkipCountdown = true;
        Skip.SetActive(false);
    }
}
