using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeSniper : Tower
{

    [Header("Changeables")]
    public int minimumDamage;

    // Start is called before the first frame update
    protected override void hitEnemy(EnemyScript damageTest)
    {
        float dist = Vector3.Distance(damageTest.gameObject.transform.position, transform.position);
        float percentage = (dist / range);
        Debug.Log((int)Mathf.Round((damageAmount - minimumDamage) * percentage) + minimumDamage);
        damageScript.TakeDamage((int)Mathf.Round((damageAmount - minimumDamage) * percentage) + minimumDamage);
    }
}
