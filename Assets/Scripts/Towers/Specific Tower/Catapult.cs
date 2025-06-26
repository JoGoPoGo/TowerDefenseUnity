using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
using UnityEngine;

public class Catapult : Tower
{
    public GameObject wurfArm;
    public GameObject stone;
    public GameObject Schale;

    public float wurfH�he;
    public float wurfWinkel;
    public float Einschlagradius;
    //public int betroffenenAnzahl = 1;

    List<GameObject> betroffenList = new List<GameObject>();
    // private Vector3 stoneEndPosition;

    // Update is called once per frame
    protected override void Shoot()
    {
        if (!spawnScript.spawned)
        {
            damageScript = target.GetComponent<DamageTest>();
            StartCoroutine(ShootAnimation());  //f�gt schaden in stoneAnimation zu

        }

    }

    protected override void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = range; // Nur Gegner **innerhalb der Reichweite** sind relevant
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            Vector3 direction = enemy.transform.position - transform.position;
            float distance = direction.magnitude;

            if (distance <= shortestDistance)
            {
                // Sichtpr�fung mit Raycast (verhindert Sch�sse durch Mauern o.�.)
                if (!Physics.Raycast(transform.position + new Vector3(0f ,0.5f * wurfH�he, 0), direction.normalized, distance, obstacleMask))
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy;
                }
                else
                {
                    Debug.Log("Attack Blocked");
                }
            }
        }

        target = nearestEnemy;
    }

    IEnumerator ShootAnimation()
    {
        float elapsed = 0f;
        float duration = 0.3f;

        Quaternion startRotation = wurfArm.transform.localRotation;
        Quaternion forwardRotation = startRotation * Quaternion.Euler(wurfWinkel, 0f, 0f);
        // stoneEndPosition = stone.transform.localPosition;

        while (elapsed < duration) //nach vorne Kippen
        {
            float t = elapsed / duration;
            wurfArm.transform.localRotation = Quaternion.Slerp(startRotation, forwardRotation, t);   
            elapsed += 3 * Time.deltaTime;
            yield return null;    //wartet einen Frame
        }
        wurfArm.transform.localRotation = forwardRotation;

        // yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(StoneAnimation());

        elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            wurfArm.transform.localRotation = Quaternion.Slerp(forwardRotation, startRotation, t);
            elapsed += (Time.deltaTime / 2);  
            yield return null;   //wartet einen Frame
        }

        wurfArm.transform.localRotation = startRotation;
    }
    IEnumerator StoneAnimation()
    {
        UpdateTarget();
        Vector3 startPosition = stone.transform.position;
        if (target == null)
        {
            Debug.Log("KEIN TARGET!! at Tower.Catapult");
            yield break;
        }
        Vector3 enemyPosition = target.transform.position;
        Vector3 peak = (startPosition + enemyPosition) / 2 + Vector3.up * wurfH�he;


        float duration = 1.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector3 pos1 = Vector3.Lerp(startPosition, peak, t);
            Vector3 pos2 = Vector3.Lerp(peak, enemyPosition, t);
            stone.transform.position = Vector3.Lerp(pos1, pos2, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        stone.transform.position = enemyPosition;
        //yield return new WaitForSeconds(0.1f);
        Vector3 schalePosition = Schale.transform.position;
        searchForInRange(stone.transform.position);   //f�gt allen Gegnern innerhalb des Einschlagradiuses schaden zu
        stone.transform.position = schalePosition;
    }
    void searchForInRange(Vector3 einschlag)
    {
       // betroffenList.Clear(); //setzt Liste Zur�ck

        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag); // Sucht nach allen Gegnern mit dem Tag "Enemy"
        //int targetetEnemies = betroffenenAnzahl;

        // Finde den n�chsten Gegner innerhalb der Reichweite
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(einschlag, enemy.transform.position);
            if (distanceToEnemy < Einschlagradius)
            {
                damageScript = enemy.GetComponent<DamageTest>();
                //betroffenList.Add(enemy);
                damageScript.TakeDamage(damageAmount);
            }
        }
    }
}
