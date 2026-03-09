using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpecial : EnemyScript
{
    public GameObject splittingPrefab;
    public int splittCount;

    public BotsOnPath pathSkript;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (isLast)
        {
            thisBotScript.totalBotsInLastWave = thisBotScript.totalBotsInLastWave + splittCount;  //fügt die Splitter als Bots hinzu
        }

    }


    // Update is called once per frame
    public override void Die(bool didDamage)
    {
        {
            if (!didDamage)
            {
                gameManager.AddCredits(reward);
            }
            for (int i = 0; i < splittCount; i++)
            {
                SpawnBot(splittingPrefab, isLast);
            }

            if (isLast)
            {
                thisBotScript.deadBotsInLastWave++;

                // Prüfen, ob ALLE Bots gespawnt und ALLE tot sind
                /*bool allFinished = true;

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
                }*/
            }
            
            isAlive = false;
            Destroy(gameObject);
        }
    }
    void SpawnBot(GameObject botPrefab, bool isLast)
    {
        Vector3 spawnPosition = pathCreator.path.GetPointAtDistance(distanceTravelled) + positionRandomizer;
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
        damageScript.distanceTravelled = distanceTravelled;
        damageScript.thisBotScript = thisBotScript;
        damageScript.otherBotScripts = otherBotScripts;
    }
}
