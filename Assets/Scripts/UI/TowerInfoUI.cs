using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerInfoUI : MonoBehaviour
{
    public GameObject panel;
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

        panel.SetActive(true);

        damageText.text = "Damage: " + tower.damageAmount.ToString();
        rangeText.text = "Range: " + tower.range.ToString();
        firerateText.text = "FIre Rate: " + tower.fireRate.ToString();
        levelText.text = "Level: " + tower.level.ToString();
        costText.text = "Cost: " + tower.upgradeCost.ToString();

    }

    public void Hide()
    {
        panel.SetActive(false);

        GameObject towerObj = currentTower.gameObject;
        DynamicRangePreview rangeSkript = towerObj.GetComponent<DynamicRangePreview>();
        rangeSkript.showActivated = false;

    }

    public void OnUpgradeButtonPressed()
    {
        if (currentTower != null)
        {
            currentTower.UpgradeTower();
            Show(currentTower);
        }

    }
}


