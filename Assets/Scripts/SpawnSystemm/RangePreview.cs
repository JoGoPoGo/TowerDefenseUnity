using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePreview : MonoBehaviour
{
    public SpawnOnMouseClick spawnScript;
    private GameObject previewObject;
    public GameObject partToActivate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        previewObject = GameObject.FindWithTag("lastSpawned");
        if (partToActivate != null)
        {
            partToActivate.SetActive(false); // Deaktiviere es zu Beginn
        }
        else
        {
            Debug.LogWarning("Kein Teilobjekt zugewiesen!");
        }
        //partToActivate = previewObject.transform.Find("Range");
        //partToActivate.SetActive(true);

    }
}
