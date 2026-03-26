using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class UpgradeSystem : MonoBehaviour
{
    [Header("Referenzes")]
    public Tower thisTower;

    [Header("Upgrade")]
    public int level;
    public int [] damageLvl;
    public float[] fireRateLvl;
    public float[] rangeLvl;
    public int[] costLvl;
    [Header("SpecialUpgrade")]
    public float[] slowerPercentageLvl;
    public float[] debuffDurationLvl;
    public float[] debuffRangeLvl;

    // Start is called before the first frame update
    public void Upgrade()
    {
        if (thisTower.level < thisTower.maxLvl)
        {
            if (thisTower.gameManager.SpendCredits((costLvl[thisTower.level])))
            {
                thisTower.level++;
                thisTower.range = rangeLvl[thisTower.level - 2]; //index 0 des Array entspricht dem ersten Upgrade, also dem Level zwei
                thisTower.damageAmount = damageLvl[thisTower.level - 2];
                thisTower.fireRate = fireRateLvl[thisTower.level - 2];

                if(thisTower is TypeDebuff)
                {
                    TypeDebuff thisTypeDebuff = thisTower as TypeDebuff;
                    thisTypeDebuff.slowerPercentage = slowerPercentageLvl[thisTower.level - 2];
                    thisTypeDebuff.debuffDuration = debuffDurationLvl[thisTower.level - 2];
                    thisTypeDebuff.debuffRange = debuffRangeLvl[thisTower.level - 2];
                }

                gameObject.transform.localScale *= 1.05f;
                thisTower.audioSource.PlayOneShot(thisTower.upgradeSound);

            }
        }
    }
}
