using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TowerUIManager : MonoBehaviour
{
    public GameObject buttonPrefab; // UI-Button-Template
    public Transform panel; // UI-Panel f�r die Buttons (mit Vertical Layout Group)

    private Dictionary<GameObject, GameObject> towerButtons = new Dictionary<GameObject, GameObject>();


    void Start()
    {
        StartCoroutine(UpdateTowerButtons());
        DeactivateAllLights();
    }

    IEnumerator UpdateTowerButtons()
    {
        while (true)
        {
            RefreshButtons();
            yield return new WaitForSeconds(1f); // Aktualisiert alle 1 Sekunde
        }
    }
    void Update()
    {
        foreach (var pair in towerButtons)
        {
            GameObject tower = pair.Key;
            GameObject button = pair.Value;

            if (tower != null && button != null)
            {
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    Tower towerComponent = tower.GetComponent<Tower>();
                    buttonText.text = $"{towerComponent.getName()} \n Cost: {towerComponent.level * towerComponent.level}";
                }
            }
        }
    }
    void RefreshButtons()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        //Debug.Log("Gefundene T�rme: " + towers.Length);

        // Neue T�rme hinzuf�gen
        foreach (GameObject tower in towers)
        {
            if (!towerButtons.ContainsKey(tower))
            {
                GameObject newButton = Instantiate(buttonPrefab, panel);
                newButton.GetComponentInChildren<TMP_Text>().text = $"{tower.GetComponent<Tower>().getName()} \n Cost: {(tower.GetComponent<Tower>().level * tower.GetComponent<Tower>().level)}";

                // Hole den EventTrigger des Buttons
                EventTrigger eventTrigger = newButton.GetComponent<EventTrigger>();
                if (eventTrigger == null) eventTrigger = newButton.AddComponent<EventTrigger>();

                // Mouse Enter (wenn die Maus �ber den Button f�hrt)
                EventTrigger.Entry entryEnter = new EventTrigger.Entry();
                entryEnter.eventID = EventTriggerType.PointerEnter;
                entryEnter.callback.AddListener((data) => OnHoverEnter(tower));  // Startet den Effekt
                eventTrigger.triggers.Add(entryEnter);

                // Mouse Exit (wenn die Maus den Button verl�sst)
                EventTrigger.Entry entryExit = new EventTrigger.Entry();
                entryExit.eventID = EventTriggerType.PointerExit;
                entryExit.callback.AddListener((data) => OnHoverExit(tower));   // Stoppt den Effekt
                eventTrigger.triggers.Add(entryExit);
                newButton.GetComponent<Button>().onClick.AddListener(() => tower.GetComponent<Tower>().UpgradeTower());
                towerButtons.Add(tower, newButton);
            }
            else
            {
                Debug.Log("Bereits ein Button f�r diesen Turm vorhanden: " + tower.name);
            }
        }

        // Entfernte T�rme l�schen
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
        Debug.Log("Maus �ber Button: " + tower.name);
        StartCoroutine(ChangeLightIntensity(tower, 100f));  // Setzt die Intensit�t des Lichts auf 1
    }

    void OnHoverExit(GameObject tower)
    {
        Debug.Log("Maus hat Button verlassen: " + tower.name);
        StartCoroutine(ChangeLightIntensity(tower, 0f));  // Setzt die Intensit�t des Lichts auf 0
    }

    // Coroutine, um die Lichtintensit�t zu ver�ndern
    IEnumerator ChangeLightIntensity(GameObject tower, float targetIntensity)
    {
        Light towerLight = tower.GetComponentInChildren<Light>(); // Hole das Licht des Turms
        if (towerLight != null)
        {
            float duration = 0f; // Dauer der Ver�nderung
            float initialIntensity = towerLight.intensity;
            float elapsedTime = 0f;

            // �ber die Zeit die Intensit�t �ndern
            while (elapsedTime < duration)
            {
                towerLight.intensity = Mathf.Lerp(initialIntensity, targetIntensity, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Endg�ltige Intensit�t setzen
            towerLight.intensity = targetIntensity;
        }
        yield return null;
    }
    // Alle Lichter zu Beginn deaktivieren
    void DeactivateAllLights()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        foreach (GameObject tower in towers)
        {
            Light towerLight = tower.GetComponentInChildren<Light>();
            if (towerLight != null)
            {
                towerLight.intensity = 0f; // Setze die Intensit�t zu Beginn auf 0
            }
        }
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void vorspulen()
    {
        Time.timeScale = 2f;
    }
    public void resetTime()
    {
        Time.timeScale = 1f;
    }

}