using UnityEngine;
using UnityEngine.UI;

public class TowerDisplayUI : MonoBehaviour
{
    public GameObject textPrefab; // Vorlage für die Textanzeige
    public Transform panel;       // Referenz zum Panel

    void Start()
    {
        DisplayTowers();
    }

    void DisplayTowers()
    {
        // Alle GameObjects mit dem Tag "Tower" finden
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        // Für jedes GameObject einen UI-Eintrag erstellen
        foreach (GameObject tower in towers)
        {
            // Neues Text-Element instanziieren
            GameObject newText = Instantiate(textPrefab, panel);

            // Namen des GameObjects anzeigen
            newText.GetComponent<Text>().text = tower.name;
        }
    }
}
