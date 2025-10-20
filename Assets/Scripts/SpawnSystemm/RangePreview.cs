using UnityEngine;

public class RangePreview : MonoBehaviour
{
    public SpawnOnMouseClick spawnScript; // Reference to the SpawnOnMouseClick script
    private GameObject previewObject;
    public GameObject partToActivate;

    void Start()
    {
        // Initialize partToActivate to null
        partToActivate = null;
    }
}

    /*void Update()
    {
        // Check if spawnScript is assigned and spawned is true
        if (spawnScript != null && spawnScript.spawned)
        {
            selectPart();
            if (partToActivate != null)
            {
                partToActivate.SetActive(true); // Activate the "Range" part
            }
            else
            {
                Debug.LogWarning("Kein Teilobjekt zugewiesen!");
            }
        }
        // Check if spawnScript is assigned and spawned is false
        else if (spawnScript != null && !spawnScript.spawned)
        {
            if (partToActivate != null)
            {
                partToActivate.SetActive(false); // Deactivate the "Range" part
            }
        }
    }

    void selectPart()
    {
        previewObject = GameObject.FindWithTag("lastSpawned");

        // Find "Range" within the prefab
        partToActivate = previewObject.transform.Find("Range").gameObject;

        // If "Range" is not found, issue a warning
        if (partToActivate == null)
        {
            Debug.LogWarning("Range Objekt im Prefab nicht gefunden!");
        }
    }
}*/
