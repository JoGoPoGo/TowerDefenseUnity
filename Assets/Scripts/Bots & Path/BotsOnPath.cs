using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class BotsOnPath : MonoBehaviour
{
    public GameObject bot1;
    public GameObject bot2;
    public GameObject bot3;

    public PathCreator pathCreator;
    public float moveSpeed = 5f;
    public float wait = 3f;
    public bool loopPath = false;

    private int botIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnBots());
    }

    // Coroutine für das Spawnen der Bots in Intervallen
    IEnumerator SpawnBots()
    {
        while (true)
        {
            GameObject botToSpawn = GetBotToSpawn();
            SpawnNewBot(botToSpawn);

            botIndex = (botIndex + 1) % 3;
            yield return new WaitForSeconds(wait);
        }
    }

    GameObject GetBotToSpawn()
    {
        switch (botIndex)
        {
            case 0: return bot1;
            case 1: return bot2;
            case 2: return bot3;
            default: return bot1;
        }
    }

    // Spawnt einen neuen Bot
    void SpawnNewBot(GameObject botPrefab)
    {
        GameObject currentBot = Instantiate(botPrefab, pathCreator.path.GetPoint(0), Quaternion.identity);
        StartCoroutine(MoveBotAlongPath(currentBot));
    }

    // Bewegung des Bots entlang des Pfads
    IEnumerator MoveBotAlongPath(GameObject enemy)
    {
        float distanceTravelled = 0f;
        DamageTest damageScript = enemy.GetComponent<DamageTest>();

        while (damageScript.isAlive)
        {
            distanceTravelled += moveSpeed * Time.deltaTime;
            enemy.transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
            enemy.transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled) * Quaternion.Euler(0, 0, 90);

            if (distanceTravelled >= pathCreator.path.length)
            {
                if (loopPath)
                {
                    distanceTravelled = 0f; // Zurück zum Start für die nächste Runde
                }
                else
                {
                    Destroy(enemy);
                    yield break;
                }
            }

            yield return null;
        }

        Destroy(enemy); // Zerstört, wenn der Bot nicht mehr "alive" ist
    }
}

