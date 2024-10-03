using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation; // Importiere PathCreation für den Pfad

public class botsOnPath : MonoBehaviour
{
    public GameObject bot;   //was? -- siehe public
    public GameObject bot2;  // zweites objekt -- siehe public
    public GameObject healthbarPrefab; //BotsHealthPrefab -- siehe public
    public PathCreator pathCreator; // Referenz auf den PathCreator
    public float moveSpeed = 5f;    // Wie schnell? -- siehe public
    public float wait = 3f;         // wie lange warten bis zum nächsten botspawn? -- siehe public
    public bool loopPath = false;   // Soll der Bot immer im Kreis laufen?

    private GameObject currentBot;  // aktuelles Objekt
    private bool useBotA = true;    // Toggle zwischen bot und bot2
    private int BotIndex = 0;

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
            GameObject botToSpawn = useBotA ? bot : bot2;
            SpawnNewBot(botToSpawn);

            // Wechsle zwischen bot und bot2
            useBotA = !useBotA;

            // Warte für die nächste Runde
            yield return new WaitForSeconds(wait);
        }
    }

    // Spawnt einen neuen Bot
    void SpawnNewBot(GameObject botPrefab)
    {
        // Erstelle eine Kopie des Bots an der Startposition des Pfads
        currentBot = Instantiate(botPrefab, pathCreator.path.GetPoint(0), Quaternion.identity);
        GameObject healthbarInstance = Instantiate(healthbarPrefab); //neue Healthbar erstellen (für bot)

        HealthSlider healthSlider = healthbarInstance.GetComponent<HealthSlider>();
        BotHealth botHealth = newBot.AddComponent<BotHealth> ();  //komponent BotHealth script
        botHealth.InitializeHealth(100, healthSlider);

        healthbarInstance.transform.SetParent(newBot.transform);
        healthbarInstance.transform.localPosition = new Vector3(0, 2f, 0);

        StartCoroutine(MoveBotAlongPath(newBot));
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
