using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation; // Importiere PathCreation für den Pfad

public class BotsOnPath : MonoBehaviour
{
    public GameObject bot1;   //was? -- siehe public
    public GameObject bot2;  // zweites objekt -- siehe public
    public GameObject bot3;   //drittes objekt -- siehe public

    public PathCreator pathCreator; // Referenz auf den PathCreator
    public float moveSpeed = 5f;    // Wie schnell? -- siehe public
    public float wait = 3f;         // wie lange warten bis zum nächsten botspawn? -- siehe public
    public bool loopPath = false;   // Soll der Bot immer im Kreis laufen?

    private GameObject currentBot;  // aktuelles Objekt
    private bool useBotA = true;    // bot und bot2
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
            GameObject botToSpawn = GetBotToSpawn(); //welcher bot?

            SpawnNewBot(botToSpawn); //spawnt bot

            botIndex = (botIndex + 1) % 3;

            // Warte für die nächste Runde
            yield return new WaitForSeconds(wait);
        }
    }
    GameObject GetBotToSpawn()
    {
        switch (botIndex)
        {
            case 0:
                return bot1;
            case 1:
                return bot2;
            case 2:
                return bot3;
            default:
                return bot1;
        }
    }

    // Spawnt einen neuen Bot
    void SpawnNewBot(GameObject botPrefab)
    {
        // Erstellt eine Kopie des Bots an der Startposition des Pfads (spawner)
        currentBot = Instantiate(botPrefab, pathCreator.path.GetPoint(0), Quaternion.identity);
        

        StartCoroutine(MoveBotAlongPath(currentBot));
    }

    // Bewegung des Bots entlang des Pfads
    IEnumerator MoveBotAlongPath(GameObject enemy)
    {
        float distanceTravelled = 0f;

        while (true)
        {
            // Bot bewegt sich entlang des Pfads
            distanceTravelled += moveSpeed * Time.deltaTime;

            // Setze die neue Position des Bots entlang des Pfads
            enemy.transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);

            // Setze die Rotation des Bots entsprechend der Richtung des Pfads
            enemy.transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled) * Quaternion.Euler(0,0,90);

            // Wenn der Bot am Ende des Pfads angekommen ist, zerstöre ihn oder mache etwas anderes
            if (!loopPath && distanceTravelled >= pathCreator.path.length)
            {
                Destroy(enemy);
                yield break; // Beende die Coroutine
            }

            yield return null;
        }
    }
}
