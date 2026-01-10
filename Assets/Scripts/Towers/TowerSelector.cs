using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSelector : MonoBehaviour
{
    private TowerInfoUI ui; // referenz auf dein UI script

    private void Start()
    {
        ui = FindObjectOfType<TowerInfoUI>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Prüft, ob der Klick auf UI fällt
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("UI geklickt – keine Welt-Interaktion");
                return; // hier abbrechen, nichts in der Welt tun
            }

            // Hier Raycast in die 3D-Welt
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Welt getroffen: " + hit.collider.name);
                Tower tower = hit.collider.GetComponentInParent<Tower>();
                if (tower != null)
                {
                    ui.Show(tower);
                }
            }
        }
    }
}

