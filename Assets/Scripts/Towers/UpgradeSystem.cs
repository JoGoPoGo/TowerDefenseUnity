using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class UpgradeSystem : MonoBehaviour
{
    [Header("Referenzes")]
    public Tower thisTower;

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

    private void Start()
    {
        thisTower = this.GetComponent<Tower>();
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

    // Start is called before the first frame update
    public void Upgrade()
    {
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
    }
}
