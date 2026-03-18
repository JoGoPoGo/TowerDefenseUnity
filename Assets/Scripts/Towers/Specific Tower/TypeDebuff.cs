using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeDebuff : TypeCanon
{
    public GameObject canonBase;

    [Header("Upgrade Variables")]

    public int slowerBool;

    [Header("Changeables")]

    public float slowerPercentage;
    public float debuffDuration;
    public float debuffRange;


    protected override void RotateTo(GameObject t)
    {
        Vector3 direction = t.transform.position - canonBase.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Quaternion smoothedRotation = Quaternion.Lerp(canonBase.transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        canonBase.transform.rotation = Quaternion.Euler(0f, smoothedRotation.eulerAngles.y, 0f);
        Debug.Log("RotateTo " + t);
    }
    protected override void hitEnemy(EnemyScript damageTest)
    {
        base.hitEnemy(damageTest);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag); // Sucht nach allen Gegnern mit dem Tag "Enemy"
        //int targetetEnemies = betroffenenAnzahl;

        // Finde den nächsten Gegner innerhalb der Reichweite
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(damageTest.gameObject.transform.position, enemy.transform.position);
            if (distanceToEnemy < debuffRange)
            {
                damageTest= enemy.GetComponent<EnemyScript>();
                if (!damageTest.isDebuffed)
                    StartCoroutine(DebuffEnemy(damageTest));
            }
        }

    }
    IEnumerator DebuffEnemy(EnemyScript enemyScript) 
    {
        enemyScript.isDebuffed = true;
        float ogSpeed = enemyScript.speed;
        enemyScript.speed *= (1f - slowerPercentage/100f);

        yield return new WaitForSeconds(debuffDuration);

        if(enemyScript != null)
        {
            enemyScript.speed = ogSpeed;
            enemyScript.isDebuffed = false;
        }
    }
    public override void UpgradeTower()
    {
        if (level < maxLvl)
        {
            if (gameManager.SpendCredits((upgradeCost)))
            {
                level++;
                if (rangeBool > 0)
                {
                    range *= (1 + (float)rangeBool / 100);
                    Debug.Log("Range Upgradet auf: " + range);
                }

                if (damageBool > 0)
                {
                    float save = damageAmount * (1 + (float)damageBool / 100);
                    damageAmount = (int)Mathf.Round(save);
                }
                if (fireRateBool > 0)
                {
                    fireRate *= (1 + (float)fireRateBool / 100);
                }

                if (cancelBool > 0)
                {
                    float save = spawnCancelRadius * (1 + (float)cancelBool / 100);
                    spawnCancelRadius = (int)Mathf.Round(save);
                }

                if(slowerBool > 0)
                {
                    float saver = slowerPercentage * (1 + (float)slowerBool / 100);
                    slowerPercentage = (int)Mathf.Round(saver);
                }

                sellReturn += (int)Mathf.Round(upgradeCost / 2);
                upgradeCost = (int)Mathf.Round((float)upgradeCost * 1.3f) + 1;  //steigert den UpgradePreis um 20%
                gameObject.transform.localScale *= 1.05f;
                rangeMinimum *= 1.05f;
                audioSource.PlayOneShot(upgradeSound);
                Debug.Log($"{gameObject.name} wurde auf Level {level} geupgradet!");
            }
        }

    }
}
