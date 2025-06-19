using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpecial : DamageTest
{
    public GameObject splittingPrefab;
    public int splittCount;

    public BotsOnPath pathSkript;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }


    // Update is called once per frame
    public override void Die(bool didDamage)
    {
        {
            if (!didDamage)
            {
                gameManager.AddCredits(reward);
            }
            
            bool splitLast = false;
            for (int i = 0; i < splittCount; i++)
            {
                if (i == splittCount && isLast)
                    splitLast = true;
                SpawnBot(splittingPrefab, splitLast);
            }

            isAlive = false;
            Destroy(gameObject);
        }
    }
    void SpawnBot(GameObject botPrefab, bool isLast)
    {
        Vector3 spawnPosition = pathCreator.path.GetPointAtDistance(distanceTravelled) + positionRandomizer;
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
    }
}
