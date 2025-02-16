using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

public class TowerUIManager : MonoBehaviour
{
    public GameObject buttonPrefab; // UI-Button-Template
    public Transform panel; // UI-Panel für die Buttons (mit Vertical Layout Group)

    private Dictionary<GameObject, GameObject> towerButtons = new Dictionary<GameObject, GameObject>();


    void Start()
    {
        StartCoroutine(UpdateTowerButtons());
    }

    IEnumerator UpdateTowerButtons()
    {
        while (true)
        {
            RefreshButtons();
            yield return new WaitForSeconds(1f); // Aktualisiert alle 1 Sekunde
        }
    }

    void RefreshButtons()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        Debug.Log("Gefundene Türme: " + towers.Length);

        // Neue Türme hinzufügen
        foreach (GameObject tower in towers)
        {
            if (!towerButtons.ContainsKey(tower))
            {
                Debug.Log("Neuer Turm gefunden: " + tower.name);
                GameObject newButton = Instantiate(buttonPrefab, panel);
                newButton.GetComponentInChildren<TMP_Text>().text = "Upgrade";
                // Hole den EventTrigger des Buttons
                EventTrigger eventTrigger = newButton.GetComponent<EventTrigger>();
                if (eventTrigger == null) eventTrigger = newButton.AddComponent<EventTrigger>();

                // Mouse Enter (wenn die Maus über den Button fährt)
                EventTrigger.Entry entryEnter = new EventTrigger.Entry();
                entryEnter.eventID = EventTriggerType.PointerEnter;
                entryEnter.callback.AddListener((data) => OnHoverEnter(tower));  // Startet den Effekt
                eventTrigger.triggers.Add(entryEnter);

                // Mouse Exit (wenn die Maus den Button verlässt)
                EventTrigger.Entry entryExit = new EventTrigger.Entry();
                entryExit.eventID = EventTriggerType.PointerExit;
                entryExit.callback.AddListener((data) => OnHoverExit(tower));   // Stoppt den Effekt
                eventTrigger.triggers.Add(entryExit);

                towerButtons.Add(tower, newButton);
            }
            else
            {
                Debug.Log("Bereits ein Button für diesen Turm vorhanden: " + tower.name);
            }
        }

        // Entfernte Türme löschen
        List<GameObject> toRemove = new List<GameObject>();
        foreach (var tower in towerButtons.Keys)
        {
            if (!System.Array.Exists(towers, t => t == tower))
            {
                Destroy(towerButtons[tower]);
                toRemove.Add(tower);
            }
        }

        foreach (var tower in toRemove)
        {
            towerButtons.Remove(tower);
        }
    }
    void OnHoverEnter(GameObject tower)
    {
        Debug.Log("Upgrade-Effekt für: " + tower.name);
        StartCoroutine(UpgradeEffect(tower)); // Starte Coroutine für den Upgrade-Effekt
    }

    // Methode, um den Effekt zu stoppen, wenn die Maus den Button verlässt
    void OnHoverExit(GameObject tower)
    {
        Debug.Log("Upgrade-Effekt gestoppt für: " + tower.name);
        // Optional: Hier könntest du den Turm sofort zurückbewegen, wenn du möchtest:
        StopCoroutine(UpgradeEffect(tower));  // Stoppt den aktuellen Effekt (optional)
        tower.transform.position = tower.transform.position - Vector3.up * 10;  // Direkt zurücksetzen
    }
    void UpgradeTower(GameObject tower)
    {
        Debug.Log("Upgrade für: " + tower.name);
        // Hier Upgrade-Logik für den Turm einfügen
        StartCoroutine(UpgradeEffect(tower));
    }
    // Coroutine für den Upgrade-Effekt (Turm wird 10 Einheiten nach oben bewegt)
    IEnumerator UpgradeEffect(GameObject tower)
    {
        float originalY = tower.transform.position.y;  // Speichere den ursprünglichen y-Wert

        Vector3 startPosition = tower.transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, originalY + 2, startPosition.z); // 2 Einheiten nach oben

        float duration = 0.2f; // Zeit für den Aufstieg
        float elapsedTime = 0;

        // Turm nach oben bewegen
        while (elapsedTime < duration)
        {
            tower.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Kurz oben bleiben
        yield return new WaitForSeconds(0.2f);

        // Zurückfallen lassen
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            tower.transform.position = Vector3.Lerp(targetPosition, startPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Setze den Turm zurück zum ursprünglichen y-Wert (falls nötig)
        tower.transform.position = new Vector3(startPosition.x, originalY, startPosition.z);
        // Stelle sicher, dass die y-Position auf 0 gesetzt wird, nach der Animation
        tower.transform.position = new Vector3(tower.transform.position.x, 0f, tower.transform.position.z);
    }


}