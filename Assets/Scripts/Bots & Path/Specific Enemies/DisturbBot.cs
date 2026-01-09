using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisturbBot : DamageTest
{
    private float updateInSeconds = 1.5f;
    private float check;
    private Tower debuffScript;

    [Header("Changeables")]

    public float disturbRange;

    public float debuffRange;
    public float debuffRate;
    public int debuffDamage;

    protected override void Start()
    {
        base.Start();
        check = updateInSeconds;
    }

    protected override void Update()
    {
        base.Update();
        check -= Time.deltaTime;
        if(check <= 0)
        {
            check = updateInSeconds;
            Disturb();
        }
    }

    private void Disturb()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers)
        {
            Vector3 direction = tower.transform.position - transform.position;
            float distance = direction.magnitude;

            if (distance < disturbRange) 
            { 
                debuffScript = tower.GetComponent<Tower>();
                ChangeValues(debuffScript);
            }
        }
    }
    private void ChangeValues(Tower towerScript)
    {
        debuffScript.range -= debuffRange;
        if(debuffScript.range <= 0) 
            debuffScript.range = 0;
        debuffScript.fireRate -= debuffRate;
        if(debuffScript.fireRate <= 0)
            debuffScript.fireRate = 0;
        debuffScript.damageAmount -= debuffDamage;
        if(debuffScript.damageAmount <= 0) 
            debuffScript.damageAmount = 0;
    }
}
