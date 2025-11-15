using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                Tower tower = hit.collider.GetComponentInParent<Tower>();
                if (tower != null)
                {
                    ui.Show(tower);
                }
            }
        }
    }
}

