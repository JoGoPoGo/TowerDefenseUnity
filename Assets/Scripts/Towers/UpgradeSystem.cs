using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using TMPro;
using Unity.VisualScripting;

public class UpgradeSystem : MonoBehaviour
{
    [Header("Referenzes")]
    public Tower thisTower;
    private TowerInfoUI towerInfo;

    [Header("Upgrade")]
    public int level;
    public List<int> damageLvl;
    public List<float> fireRateLvl;
    public List<float> rangeLvl;
    public List<int> costLvl;

    [Header("SpecialUpgrade")]
    public List<float> slowerPercentageLvl;
    public List<float> debuffDurationLvl;
    public List<float> debuffRangeLvl;

    public List<GameObject> switchPrefabsLvl3;
    public int switchIntiger = 0;

    [Header("Funktion")]
    public bool isCurrentInfo = false;
    private bool refresher = true;

    private void Start()
    {
        thisTower = this.GetComponent<Tower>();
        towerInfo = FindObjectOfType<TowerInfoUI>();
        if (costLvl == null)
        {
            costLvl = new List<int>() { thisTower.price, thisTower.price * 2, thisTower.price * 3 };
        }

        int longestLength = 0;

        if (damageLvl.Count > longestLength)
            longestLength = damageLvl.Count;
        if (fireRateLvl.Count > longestLength)
            longestLength = fireRateLvl.Count;
        if (rangeLvl.Count > longestLength)
            longestLength = rangeLvl.Count;
        if (costLvl.Count > longestLength)
            longestLength = costLvl.Count;

        // Alle Listen auf gleiche Länge bringen
        while (damageLvl.Count < longestLength)
        {
            damageLvl.Add(0);
        }

        while (fireRateLvl.Count < longestLength)
        {
            fireRateLvl.Add(0f);
        }

        while (rangeLvl.Count < longestLength)
        {
            rangeLvl.Add(0f);
        }

        while (costLvl.Count < longestLength)
        {
            costLvl.Add(0);
        }

    }

    private void Update()
    {
        if(isCurrentInfo && refresher)
        {
            refresher = false;
            UpgradeButton buttonScript;
            towerInfo.upgradeButton1.gameObject.SetActive(false);
            towerInfo.upgradeButton2.gameObject.SetActive(false);
            towerInfo.upgradeButton3.gameObject.SetActive(false);
            if(thisTower.level == 2)
            {
                towerInfo.upgradeButton1.gameObject.SetActive(switchPrefabsLvl3.Count > 0);
                buttonScript = towerInfo.upgradeButton1.GetComponent<UpgradeButton>();
                buttonScript.upgradeSystem = this;
                towerInfo.upgradeButton2.gameObject.SetActive(switchPrefabsLvl3.Count > 1);
                buttonScript = towerInfo.upgradeButton2.GetComponent<UpgradeButton>();
                buttonScript.upgradeSystem = this;
                towerInfo.upgradeButton3.gameObject.SetActive(switchPrefabsLvl3.Count > 2);
                buttonScript = towerInfo.upgradeButton3.GetComponent<UpgradeButton>();
                buttonScript.upgradeSystem = this;
            }

        }
        else
        {
            refresher= true;
        }
    }

    // Start is called before the first frame update
    public void Upgrade(int number = -1)
    {
        switchIntiger = number;
        int upgradeIndex = thisTower.level - 1;
        // Prüfen ob es überhaupt dieses Upgrade gibt
        if (upgradeIndex >= costLvl.Count)
        {
            Debug.Log("Kein weiteres Upgrade vorhanden.");
            return;
        }

        // Kosten holen
        int upgradeCost = costLvl[upgradeIndex];

        if (thisTower.gameManager.SpendCredits(upgradeCost))
        {
            Debug.Log("UpgradeSystem");
            thisTower.level++;

            if(thisTower.level == 3 && switchPrefabsLvl3.Count >= 0)
            {
                switchPrefabs();
                return;
            }
            if (upgradeIndex < rangeLvl.Count)
            {
                thisTower.range = rangeLvl[upgradeIndex];
                thisTower.damageAmount = damageLvl[upgradeIndex];
                thisTower.fireRate = fireRateLvl[upgradeIndex];
            }

            if (thisTower is TypeDebuff thisTypeDebuff)
            {
                if (upgradeIndex < slowerPercentageLvl.Count)
                {
                    thisTypeDebuff.slowerPercentage = slowerPercentageLvl[upgradeIndex];
                    thisTypeDebuff.debuffDuration = debuffDurationLvl[upgradeIndex];
                    thisTypeDebuff.debuffRange = debuffRangeLvl[upgradeIndex];
                }

            }

            gameObject.transform.localScale *= 1.05f;
            thisTower.audioSource.PlayOneShot(thisTower.upgradeSound);
        }
        if (towerInfo.infoText.activeSelf)
            towerInfo.Show(thisTower);
        refresher = true;
    }
    public void switchPrefabs()
    {
        Vector3 pos = gameObject.transform.position;
        Quaternion rot = Quaternion.identity;
        GameObject newTower = Instantiate(switchPrefabsLvl3[switchIntiger], pos, rot);
        newTower.layer = LayerMask.NameToLayer("Default");
        thisTower.price = 0;
        thisTower.Sell();
    }
}
