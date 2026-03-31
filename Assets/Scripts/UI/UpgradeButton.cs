using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    // Start is called before the first frame update
    private Button button;
    public int number;
    public UpgradeSystem upgradeSystem;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        upgradeSystem.Upgrade(number);
    }
}
