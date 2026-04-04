using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using TMPro;
using Unity.VisualScripting;

[Serializable]
public class TowerUpgradeData
{
    [Header("Standardwerte")]
    public int damage;
    public float fireRate;
    public float range;
    public int cost;

    [Header("Optional: Debuffwerte")]
    public float slowerPercentage;
    public float debuffDuration;
    public float debuffRange;

    [Header("Optional: Tower-Wechsel")]
    public List<GameObject> switchPrefabs;
}

public class UpgradeSystem : MonoBehaviour
{
    [Header("Referenzen")]
    public Tower thisTower;
    private TowerInfoUI towerInfo;

    [Header("Upgrade")]
    public int level;
    public List<TowerUpgradeData> upgrades = new List<TowerUpgradeData>();

    [Header("Funktion")]
    public bool isCurrentInfo = false;
    private bool refresher = true;

    private int selectedUpgradeBranch = 0;

    private void Start()
    {
        thisTower = GetComponent<Tower>();
        towerInfo = FindObjectOfType<TowerInfoUI>();

        // Falls keine Upgrades im Inspector gesetzt wurden
        if (upgrades == null || upgrades.Count == 0)
        {
            upgrades = new List<TowerUpgradeData>()
            {
                new TowerUpgradeData()
                {
                    damage = thisTower.damageAmount,
                    fireRate = thisTower.fireRate,
                    range = thisTower.range,
                    cost = thisTower.price
                }
            };
        }
    }

    private void Update()
    {
        if (isCurrentInfo && refresher)
        {
            refresher = false;

            UpgradeButton buttonScript;

            towerInfo.upgradeButton1.gameObject.SetActive(false);
            towerInfo.upgradeButton2.gameObject.SetActive(false);
            towerInfo.upgradeButton3.gameObject.SetActive(false);

            int upgradeIndex = thisTower.level - 1;

            // Prüfen ob überhaupt ein nächstes Upgrade existiert
            if (upgradeIndex >= 0 && upgradeIndex < upgrades.Count)
            {
                TowerUpgradeData currentUpgrade = upgrades[upgradeIndex];

                // Nur wenn dieses Upgrade Switch-Prefabs besitzt
                if (currentUpgrade.switchPrefabs != null && currentUpgrade.switchPrefabs.Count > 0)
                {
                    towerInfo.upgradeButton1.gameObject.SetActive(currentUpgrade.switchPrefabs.Count > 0);
                    buttonScript = towerInfo.upgradeButton1.GetComponent<UpgradeButton>();
                    buttonScript.upgradeSystem = this;

                    towerInfo.upgradeButton2.gameObject.SetActive(currentUpgrade.switchPrefabs.Count > 1);
                    buttonScript = towerInfo.upgradeButton2.GetComponent<UpgradeButton>();
                    buttonScript.upgradeSystem = this;

                    towerInfo.upgradeButton3.gameObject.SetActive(currentUpgrade.switchPrefabs.Count > 2);
                    buttonScript = towerInfo.upgradeButton3.GetComponent<UpgradeButton>();
                    buttonScript.upgradeSystem = this;
                }
            }
        }
        else
        {
            refresher = true;
        }
    }

    public void Upgrade(int number = 0)
    {
        selectedUpgradeBranch = number;

        int upgradeIndex = thisTower.level - 1;

        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Count)
        {
            Debug.Log("Kein weiteres Upgrade vorhanden.");
            return;
        }

        TowerUpgradeData currentUpgrade = upgrades[upgradeIndex];

        if (thisTower.gameManager.SpendCredits(currentUpgrade.cost))
        {
            Debug.Log("UpgradeSystem");
            thisTower.level++;

            // Spezialfall: Tower-Wechsel auf Level 3
                List<TowerUpgradeData> branchUpgrades = GetBranchUpgradesForLevel3();

                if (branchUpgrades[thisTower.level].switchPrefabs.Count != 0)
                {
                    SwitchPrefab(branchUpgrades[thisTower.level--].switchPrefabs[number]);
                    return;
                }

            // Normale Werte anwenden
            thisTower.range = currentUpgrade.range;
            thisTower.damageAmount = currentUpgrade.damage;
            thisTower.fireRate = currentUpgrade.fireRate;

            // Debuff-Werte anwenden, falls Debuff-Tower
            if (thisTower is TypeDebuff thisTypeDebuff)
            {
                thisTypeDebuff.slowerPercentage = currentUpgrade.slowerPercentage;
                thisTypeDebuff.debuffDuration = currentUpgrade.debuffDuration;
                thisTypeDebuff.debuffRange = currentUpgrade.debuffRange;
            }

            transform.localScale *= 1.05f;
            thisTower.audioSource.PlayOneShot(thisTower.upgradeSound);
        }

        if (towerInfo.infoText.activeSelf)
            towerInfo.Show(thisTower);

        refresher = true;
    }

    private List<TowerUpgradeData> GetBranchUpgradesForLevel3()
    {
        List<TowerUpgradeData> branches = new List<TowerUpgradeData>();

        foreach (TowerUpgradeData upgrade in upgrades)
        {
            if (upgrade.switchPrefabs.Count != 0)
                branches.Add(upgrade);
        }

        return branches;
    }

    public void SwitchPrefab(GameObject prefabToSpawn)
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        GameObject newTower = Instantiate(prefabToSpawn, pos, rot);
        newTower.layer = LayerMask.NameToLayer("Default");

        thisTower.price = 0;
        thisTower.Sell();
    }
}