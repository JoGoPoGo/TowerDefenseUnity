using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preview : MonoBehaviour
{
    public SpawnOnMouseClick spawnScript;
    private GameObject previewObject; // Das Objekt, das als Vorschau angezeigt wird

    void start()
    {
        previewObject = spawnScript.selectedPrefab;
    }
    // Update wird einmal pro Frame aufgerufen
    void Update()
    {

        // Überprüfe, ob ein Objekt gespawnt werden soll
        if (spawnScript.spawned && previewObject != null && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Wenn der Raycast etwas trifft
            if (Physics.Raycast(ray, out hit))
            {
                // Überprüfe, ob die getroffene Oberfläche sich am Boden (y = 0) befindet
                if (hit.point.y == 0)
                {
                    // Setze die Position des Vorschau-Objekts auf den Trefferpunkt
                    previewObject.transform.position = new Vector3(hit.point.x, 0, hit.point.z);

                    // Optional: Drehung des Vorschau-Objekts anpassen, um mit der Mausbewegung auszurichten
                    previewObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                }
            }
        }
    }
}
