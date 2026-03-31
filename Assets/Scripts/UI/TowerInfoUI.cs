using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TowerInfoUI : MonoBehaviour
{
    public UIPanelController panel;
    public TMP_Text damageText;
    public TMP_Text rangeText;
    public TMP_Text firerateText;
    public TMP_Text levelText;
    public TMP_Text costText;
    public GameObject infoText;
    public TMP_Text upgradeText;

    public Button upgradeButton1;
    public Button upgradeButton2;
    public Button upgradeButton3;

    private Tower currentTower;
    private UpgradeSystem currentUpgradeSystem;

    public void switchInfoActive()
    {
        infoText.SetActive(!infoText.activeSelf);
    }

    public void Show(Tower tower)
    {
        if (currentTower != null)
        {
            currentUpgradeSystem.isCurrentInfo = false;
        }
        currentTower = tower;
        GameObject towerObj = tower.gameObject;
        currentUpgradeSystem = towerObj.GetComponent<UpgradeSystem>();
        currentUpgradeSystem.isCurrentInfo = true;
        DynamicRangePreview rangeSkript = towerObj.GetComponent<DynamicRangePreview>();
        rangeSkript.showActivated = true;

        if (panel != null) panel.Open();
        //Debug.Log("Show");

        damageText.text = "Damage: " + tower.damageAmount.ToString();
        rangeText.text = "Range: " + tower.range.ToString();
        firerateText.text = "FIre Rate: " + ((Mathf.Round(tower.fireRate * 100))/100).ToString();
        levelText.text = "Level: " + tower.level.ToString();

    }

    public void Hide()            //Versteckt die Rangepreview über das Deaktivieren von showActivated in DynamicRangePreview des Turms
    {
        if (panel != null) panel.Close();

        if (currentTower != null)
        {
            GameObject towerObj = currentTower.gameObject;
            currentUpgradeSystem = towerObj.GetComponent<UpgradeSystem>();
            DynamicRangePreview rangeSkript = towerObj.GetComponent<DynamicRangePreview>();
            rangeSkript.showActivated = false;
        }
    }

    public void OnUpgradeButtonPressed()
    {
        if (currentTower != null)
        {
            currentUpgradeSystem.Upgrade();
            Show(currentTower);
        }

    }

    public void OnSellButtonPressed()
    {
        if(currentTower != null)
        {
            Tower towerScript = currentTower.GetComponent<Tower>();
            towerScript.Sell();
        }
    }
}


