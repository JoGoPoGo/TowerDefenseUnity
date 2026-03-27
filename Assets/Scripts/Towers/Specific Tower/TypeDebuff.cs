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
}
