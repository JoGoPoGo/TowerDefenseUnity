using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPanelOnE : MonoBehaviour
{
    public GameObject handleObject;
    public KeyCode onKey;

    private UIPanelController panelController;

    private void Start()
    {
        panelController = GetComponent<UIPanelController>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(onKey) && handleObject != null)
        {
            handleObject.SetActive(!handleObject.activeSelf);
        }
    }
}
