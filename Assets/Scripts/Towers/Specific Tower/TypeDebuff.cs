using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeDebuff : TypeCanon
{
    public GameObject canonBase;

    [Header("Changeables")]

    public float slowerPercentage;
    public float debuffDuration;


    protected override void RotateTo(GameObject t)
    {
        Vector3 direction = t.transform.position - canonBase.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Quaternion smoothedRotation = Quaternion.Lerp(canonBase.transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        canonBase.transform.rotation = Quaternion.Euler(0f, smoothedRotation.eulerAngles.y, 0f);
    }
    protected override void hitEnemy(EnemyScript damageTest)
    {
        base.hitEnemy(damageTest);
        if(!damageTest.isDebuffed)
            StartCoroutine(DebuffEnemy(damageTest));
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
