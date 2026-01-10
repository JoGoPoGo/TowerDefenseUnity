using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisturbBot : DamageTest
{
    private float updateInSeconds = 3f;
    private float check;
    private HashSet<Tower> disturbedTowers = new HashSet<Tower>();  //alle Türme, die gerade gedebuff sind
    private HashSet<Tower> normalTowers = new HashSet<Tower>(); //alle Türme, die NICHT MEHR in disturbedTowers sind

    [Header("Changeables")]

    public float disturbrange;

    public float debuffRange;
    public float debuffRate;
    public float debuffDamage;

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

        normalTowers.Clear(); // reseted normalTowers

        foreach (GameObject towerGO in towers)
        {
            Tower tower = towerGO.GetComponent<Tower>();
            if (tower == null)
                continue;

            Vector3 direction = towerGO.transform.position - transform.position;
            float distance = direction.magnitude;

            if (distance < disturbrange)   //wenn der Turm In Reichweite ist
            {
                if (!disturbedTowers.Contains(tower))        //wenn der Turm noch nicht debuffed wurde
                {
                    DebuffValues(tower);
                    DisturbAnimation(towerGO);
                    disturbedTowers.Add(tower);
                }
            }
            else if (disturbedTowers.Contains(tower))      //wenn der Turm außerhalb der Reichweite ist und schon debuffed wurde...
            {
                normalTowers.Add(tower);
                disturbedTowers.Remove(tower);              //...wird er von disturbedTowers entfernt
                ResetValues(tower);
                StopDisturbAnimation(towerGO);
            }

        }
    }
    private void DebuffValues(Tower tower)
    {
        tower.range = Mathf.Max(0, tower.range * debuffRange);
        tower.fireRate = Mathf.Max(0, tower.fireRate * debuffRate);
        tower.damageAmount = (int)Mathf.Round(Mathf.Max(0, tower.damageAmount * debuffDamage));
    }

    private void ResetValues(Tower tower)
    {
        tower.range = Mathf.Max(0, tower.range / debuffRange);
        tower.fireRate = Mathf.Max(0, tower.fireRate / debuffRate);
        tower.damageAmount = (int)Mathf.Round(Mathf.Max(0, tower.damageAmount / debuffDamage));
    }

    public override void Die(bool didDamage)
    {
        foreach(Tower tower in disturbedTowers)
        {
            ResetValues(tower);
            GameObject GO = tower.gameObject;
            StopDisturbAnimation(GO);
        }
        disturbedTowers.Clear();
        base.Die(didDamage);
    }

    private void DisturbAnimation(GameObject towerGo)
    {
        Tower tower = towerGo.GetComponent<Tower>();
        if (tower == null) return;

        tower.PlayDisturb();
    }
    private void StopDisturbAnimation(GameObject towerGo)
    {
        Tower tower = towerGo.GetComponent<Tower>();
        if (tower == null) return;

        tower.StopDisturb();
    }
}
