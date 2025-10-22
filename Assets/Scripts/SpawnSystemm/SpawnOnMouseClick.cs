using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnOnMouseClick : MonoBehaviour
{
    public bool spawned = false;
    public GameObject[] prefabsToSpawn; // Array von GameObjects, die gespawnt werden k�nnen
    public Button[] spawnButtons; // Array von Buttons, die die Auswahl der GameObjects steuern
    public int selectedPrefabIndex = 0; // Index des aktuell ausgew�hlten GameObjects
    private bool spawnEnabled = false; // Frag, ob das Spawning aktiviert ist 
    public GameObject selectedPrefab;
    public Preview previewScript;

    //Tims �nderung
    public GameManager gameManager;
    public int cost = 0;       //wie viel Kostet der Turm?

    void Start()
    {
        // Initialisierung der Buttons:
        for (int i = 0; i < spawnButtons.Length; i++)
        {
            int buttonIndex = i; // Speichern des Button-Index f�r den Event-Handler

            Tower towerScript = prefabsToSpawn[i].GetComponent<Tower>();
            string towerName = towerScript != null ? towerScript.name : "Unbekannt";
            int towerPrice = towerScript != null ? towerScript.price : 0;

            // TextMeshPro-Textfeld finden und aktualisieren
            TextMeshProUGUI buttonText = spawnButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = $"{towerName}\n cost: {towerPrice}";
            }


            spawnButtons[i].onClick.AddListener(() => {
                selectedPrefabIndex = buttonIndex; // Setze den ausgew�hlten Index, wenn der Button gedr�ckt wird
                spawnEnabled = true; // Aktiviere das Spawning
                //Tims �nderung
                UpdateCost();

            });
        }
    }

    void Update()
    {
        // Wenn das Spawning aktiviert ist und die linke Maustaste gedr�ckt wird
        if (spawnEnabled && Input.GetMouseButtonDown(0))
        {
            if (gameManager.SpendCredits(cost))            // Tims �nderung  -- wenn der Preis bezahlbar ist.
            {
                
                // Raycast von der Kamera zum Mauszeiger
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // Wenn der Raycast etwas trifft
                if (Physics.Raycast(ray, out hit)/*&& previewScript.IsPositionValid()*/)
                {
                    Vector3 placePosition = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));

                    if (previewScript.IsPositionValidAt(hit.point))
                    {

                        // Spawnen des ausgew�hlten GameObjects an der Hit-Position
                        GameObject spawnedObject = Instantiate(prefabsToSpawn[selectedPrefabIndex], placePosition /*hit.point*/, Quaternion.identity);

                        // Setze den Namen und Tag des neu erstellten Objekts
                        selectedPrefab = spawnedObject;
                        spawnedObject.name = "Tower";
                        spawnedObject.tag = "lastSpawned";

                        spawned = true;
                    }
                    else
                    {
                        gameManager.AddCredits(cost);
                    }

                }

                // Deaktiviere das Spawning nach dem Spawnen
                spawnEnabled = false;
            }
            
        }
        
    }
    void UpdateCost()                 //Tims �nderung -- gibt den Korekten preis zur�ck
    {
        Tower towerScript = prefabsToSpawn[selectedPrefabIndex].GetComponent<Tower>();
        cost = towerScript.price;
    }
}
