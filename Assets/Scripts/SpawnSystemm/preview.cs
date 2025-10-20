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

    public Material normalMaterial;    // Material f�r g�ltige Position
    public Material sperrbereichMaterial; // Material f�r ung�ltige Position

    void Start()
    {
        //previewObject = spawnScript.selectedPrefab;
    }
    // Update wird einmal pro Frame aufgerufen
    void Update()
    {
        previewObject = GameObject.FindWithTag("lastSpawned");

        // �berpr�fe, ob ein Objekt gespawnt werden soll
        if (spawnScript.spawned && previewObject != null && Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Wenn der Raycast etwas trifft
            if (Physics.Raycast(ray, out hit))
            {
                hitpointx = hit.point.x;
                hitpointz = hit.point.z;
                if (IsPositionValidAt(new Vector3(hitpointx, 0, hitpointz)))
                {
                    //SetVisibility(previewObject, true);
                    previewObject.transform.position = new Vector3(Mathf.Round(hit.point.x), 0, Mathf.Round(hit.point.z));
                    ChangeRangeMaterial(normalMaterial);
                }
                

                
                // Setze die Position des Vorschau-Objekts auf den Trefferpunkt
                else
                {
                    Debug.Log("Sperrbereich");
                    //SetVisibility(previewObject, false);
                    ChangeRangeMaterial(sperrbereichMaterial);
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
    public bool IsPositionValidAt(Vector3 position)
    {
        Tower towerPrefab = spawnScript.prefabsToSpawn[spawnScript.selectedPrefabIndex].GetComponent<Tower>();
        float ownCancelRadius = towerPrefab.spawnCancelRadius;
        // Suche nach T�rmen in der Szene
        Tower[] towers = FindObjectsOfType<Tower>();
        foreach (Tower tower in towers)
        {
            if (tower.CompareTag("lastSpawned"))
            {
                continue; // Gehe zum n�chsten Turm
            }
            float distance = Vector3.Distance(new Vector3(position.x, 0, position.z), tower.transform.position);
            if (distance < tower.spawnCancelRadius || distance < ownCancelRadius)
            {
                return false; // Position ist innerhalb des No-Tower-Bereichs
            }
        }
        return true; // Position ist g�ltig
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
    void ChangeRangeMaterial(Material material)
    {
        if (previewObject == null) return;

        Transform rangeChild = previewObject.transform.Find("Range"); // Suche nach dem Child-Objekt "Range"
        if (rangeChild != null)
        {
            Renderer rangeRenderer = rangeChild.GetComponent<Renderer>();
            if (rangeRenderer != null)
            {
                rangeRenderer.material = material; // Material �ndern
            }
        }
    }
}
