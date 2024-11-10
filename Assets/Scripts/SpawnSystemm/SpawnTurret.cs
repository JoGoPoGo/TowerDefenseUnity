using UnityEngine;
using UnityEngine.UI;

public class SpawnOnMouseClick : MonoBehaviour
{
    public bool spawned = false;
    public GameObject[] prefabsToSpawn; // Array von GameObjects, die gespawnt werden können
    public Button[] spawnButtons; // Array von Buttons, die die Auswahl der GameObjects steuern
    private int selectedPrefabIndex = 0; // Index des aktuell ausgewählten GameObjects
    private bool spawnEnabled = false; // Flag, ob das Spawning aktiviert ist
    public GameObject selectedPrefab;
    void Start()
    {
        // Initialisierung der Buttons:
        for (int i = 0; i < spawnButtons.Length; i++)
        {
            int buttonIndex = i; // Speichern des Button-Index für den Event-Handler
            spawnButtons[i].onClick.AddListener(() => {
                selectedPrefabIndex = buttonIndex; // Setze den ausgewählten Index, wenn der Button gedrückt wird
                spawnEnabled = true; // Aktiviere das Spawning
            });
        }
    }

    void Update()
    {
    

        // Wenn das Spawning aktiviert ist und die linke Maustaste gedrückt wird
        if (spawnEnabled && Input.GetMouseButtonDown(0))
        {
            // Raycast von der Kamera zum Mauszeiger
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Wenn der Raycast etwas trifft
            if (Physics.Raycast(ray, out hit))
            {
                selectedPrefab = prefabsToSpawn[selectedPrefabIndex];
                // Spawnen des ausgewählten GameObjects an der Hit-Position
                Instantiate(selectedPrefab, hit.point + new Vector3(0, 1, 0), Quaternion.identity);
                spawned = true;
            }

            // Deaktiviere das Spawning nach dem Spawnen
            spawnEnabled = false;
        }
    }
}
