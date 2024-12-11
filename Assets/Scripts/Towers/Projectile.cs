using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private bool targetLifes = true;
    private float shootSpeed = 5f;
    private int damage = 10;

    private GameObject nearestTower;
    private Vector3 startPosition;
    private Transform target;
    private float range = 0f;
    private DamageTest damageScript;

    void Start()
    {
        startPosition = transform.position;

        nearestTower = FindNearestTower();
        range = nearestTower.GetComponent<Tower>().range;
        shootSpeed = nearestTower.GetComponent<Tower>().bulletSpeed;
        damage = nearestTower.GetComponent<Tower>().damageAmount;

        UpdateTarget();
    }

    void Update()
    {
        // Prüfen, ob ein Ziel vorhanden ist
        if (target == null || damageScript == null || !damageScript.isAlive)
        {
            UpdateTarget();

            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
        }

        // Bewegung des Projektils in Richtung Ziel
        transform.position = Vector3.MoveTowards(transform.position, target.position, shootSpeed * Time.deltaTime);

        // Prüfen, ob das Projektil das Ziel erreicht hat
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            damageScript.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            Transform centerTransform = nearestEnemy.transform.Find("Center");

            if (centerTransform != null)
            {
                target = centerTransform;
                damageScript = nearestEnemy.GetComponent<DamageTest>();
            }
            else
            {
                target = nearestEnemy.transform; // Fallback falls "Center" fehlt
                damageScript = nearestEnemy.GetComponent<DamageTest>();
            }
        }
        else
        {
            target = null;
        }
    }

    GameObject FindNearestTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestTower = null;

        foreach (GameObject tower in towers)
        {
            float distanceToTower = Vector3.Distance(transform.position, tower.transform.position);
            if (distanceToTower < shortestDistance)
            {
                shortestDistance = distanceToTower;
                nearestTower = tower;
            }
        }
        Debug.Log("Tower Found");
        return nearestTower;
    }
}



