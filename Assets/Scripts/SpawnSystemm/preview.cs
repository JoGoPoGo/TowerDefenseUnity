using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preview : MonoBehaviour
{
    public SpawnOnMouseClick spawnScript;
    private GameObject previewObject; // Das Objekt, das als Vorschau angezeigt wird

    void start()
    {
        //previewObject = spawnScript.selectedPrefab;
    }
    // Update wird einmal pro Frame aufgerufen
    void Update()
    {
        previewObject = GameObject.FindWithTag("lastSpawned");

        // Überprüfe, ob ein Objekt gespawnt werden soll
        if (spawnScript.spawned && previewObject != null && Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Wenn der Raycast etwas trifft
            if (Physics.Raycast(ray, out hit))
            {
                    
                
                    // Setze die Position des Vorschau-Objekts auf den Trefferpunkt
                    previewObject.transform.position = new Vector3(hit.point.x, 3, hit.point.z)+ new Vector3(0, 0, 2);
                   

                    // Optional: Drehung des Vorschau-Objekts anpassen, um mit der Mausbewegung auszurichten
                    // previewObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                
            }
        }
    }
}
