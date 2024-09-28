using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderSpawner : MonoBehaviour
{
    public GameObject bot;   //was? -- siehe public
    public GameObject bot2;    //zweites objekt -- siehe public
    public Transform target;   //wohin? -- siehe public
    
    public float moveSpeed = 2f;       // Wie schnell? -- siehe public
    public float wait = 3f; //wie lange warten bis zum nächsten botspawn? --siehe public

    private GameObject currentBot; // aktuelles objekt
    private GameObject currentBot2; //aktuelles objekt zwei
    private Vector3 spawnPosition;// Wo?

    private bool useBotA = true;
    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = transform.position;
        StartCoroutine(SpawnBots());
        
    }

    IEnumerator SpawnBots()
    {
        while (true)
        {
            GameObject botToSpawn = useBotA ? bot : bot2;
            SpawnNewBot(botToSpawn);

            useBotA = !useBotA;

            yield return new WaitForSeconds(wait);
        }
    }

    // Spawnt einen neuen Bot
    void SpawnNewBot(GameObject botPrefab)
    {
        // erstellt Kopie des Bots am angegebenen Punkt
        currentBot = Instantiate(botPrefab, spawnPosition, Quaternion.identity);
        StartCoroutine(MoveBot(currentBot));
    }

    // Zum Target und zerstört ihn nach dem erreichen des Ziels
    IEnumerator MoveBot(GameObject Enemy)
    {
        while (Vector3.Distance(Enemy.transform.position, target.position) > 0.1)
        {
            Enemy.transform.position = Vector3.MoveTowards(Enemy.transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Zerstöre den Zylinder, wenn er das Ziel erreicht hat
        Destroy(Enemy);
    }

    //update Methode
    void Update()
    {
        
    }
}
