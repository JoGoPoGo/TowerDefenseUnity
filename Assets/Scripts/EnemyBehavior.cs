using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderSpawner : MonoBehaviour
{
    public GameObject bot;   //was? -- siehe public
    
    public float moveDistance = 8f;    // Wie weit läuft er? -- siehe public
    public float moveSpeed = 2f;       // Wie schnell? -- siehe public

    private GameObject currentBot; // aktuelles objekt
    private Vector3 spawnPosition;// Wo?
    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = transform.position;
        SpawnNewBot(); // Spawne den Bot
        
    }

    // Spawnt einen neuen Bot
    void SpawnNewBot()
    {
        // Spawne den Zylinder am angegebenen Punkt
        currentBot = Instantiate(bot, spawnPosition, Quaternion.identity);
        StartCoroutine(MoveBot(currentBot));
    }

    // Bewegt den Zylinder nach vorne und zerstört ihn, sobald er die Distanz erreicht hat
    IEnumerator MoveBot(GameObject Enemy)
    {
        Vector3 startPosition = Enemy.transform.position;
        Vector3 endPosition = startPosition + Enemy.transform.forward * moveDistance;

        // Bewege den Zylinder nach vorne
        while (Vector3.Distance(Enemy.transform.position, endPosition) > 0.1f)
        {
            Enemy.transform.position = Vector3.MoveTowards(Enemy.transform.position, endPosition, moveSpeed * Time.deltaTime);
            yield return null;  // Warte bis zum nächsten Frame
        }

        // Zerstöre den Zylinder, wenn er das Ziel erreicht hat
        Destroy(Enemy);

        // Warte bis der nächste Bot spawnt
        yield return new WaitForSeconds(0.1f);

        // Spawne neuen Bot
        SpawnNewBot();
    }

    //update Methode
    void Update()
    {
        
    }
}
