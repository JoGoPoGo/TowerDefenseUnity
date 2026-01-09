using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerInfoUI : MonoBehaviour
{
    public UIPanelController panel;
    public TMP_Text damageText;
    public TMP_Text rangeText;
    public TMP_Text firerateText;
    public TMP_Text levelText;
    public TMP_Text costText;

    private Tower currentTower;

    public void Show(Tower tower)
    {
        currentTower = tower;
        GameObject towerObj = tower.gameObject;
        DynamicRangePreview rangeSkript = towerObj.GetComponent<DynamicRangePreview>();
        rangeSkript.showActivated = true;

        if (panel != null) panel.Open();
        //Debug.Log("Show");

        damageText.text = "Damage: " + tower.damageAmount.ToString();
        rangeText.text = "Range: " + tower.range.ToString();
        firerateText.text = "FIre Rate: " + tower.fireRate.ToString();
        levelText.text = "Level: " + tower.level.ToString();
        costText.text = "Cost: " + tower.upgradeCost.ToString();

    }

    public void Hide()            //Versteckt die Rangepreview über das Deaktivieren von showActivated in DynamicRangePreview des Turms
    {
        if (panel != null) panel.Close();

        if (currentTower != null)
        {
            GameObject towerObj = currentTower.gameObject;
            DynamicRangePreview rangeSkript = towerObj.GetComponent<DynamicRangePreview>();
            rangeSkript.showActivated = false;
        }
    }

    public void OnUpgradeButtonPressed()
    {
        if (currentTower != null)
        {
            currentTower.UpgradeTower();
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


