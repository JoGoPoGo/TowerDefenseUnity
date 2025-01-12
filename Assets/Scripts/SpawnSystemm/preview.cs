using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Preview : MonoBehaviour
{
    public SpawnOnMouseClick spawnScript;
    private GameObject previewObject; // Das Objekt, das als Vorschau angezeigt wird
    private float hitpointx;
    private float hitpointz;

    void Start()
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
                hitpointx = hit.point.x;
                hitpointz = hit.point.z;
                if (IsPositionValid())
                {
                    SetVisibility(previewObject, true);
                    previewObject.transform.position = new Vector3(hit.point.x, 3, hit.point.z);
                }
                

                
                // Setze die Position des Vorschau-Objekts auf den Trefferpunkt
                else
                {
                    Debug.Log("Sperrbereich");
                    SetVisibility(previewObject, false);
                }

                    
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            SetVisibility(previewObject, true);
            spawnScript.spawned = false; // Setze spawned auf false, wenn die linke Maustaste losgelassen wird

            if (previewObject != null)
            {
                previewObject.tag = "Tower";
            }

        }
        
    }
    public bool IsPositionValid()
    {

        // Suche nach Türmen in der Szene
        Tower[] towers = FindObjectsOfType<Tower>();
        foreach (Tower tower in towers)
        {
            if (tower.CompareTag("lastSpawned"))
            {
                continue; // Gehe zum nächsten Turm
            }
            float distance = Vector3.Distance(new Vector3(hitpointx, 3, hitpointz), tower.transform.position);
            if (distance < tower.spawnCancelRadius)
            {
                return false; // Position ist innerhalb des No-Tower-Bereichs
            }
        }
        return true; // Position ist gültig
    }
    void SetVisibility(GameObject obj, bool visible)
    {
        if (obj == null)
        {
            return;
        }
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = visible;
        }
    }
}
