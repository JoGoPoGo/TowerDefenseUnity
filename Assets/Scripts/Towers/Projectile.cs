using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class Projectile : MonoBehaviour
{
    private bool targetLifes = true;
    private float shootSpeed = 5f;
    private int damage = 10;

    //private GameObject tower;
    private GameObject nearestTower;
    private Vector3 startPosition;
    private Transform target;
    //private GameObject targetEnemy;
    private float range = 0f;
    private DamageTest damageScript;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;

        //targetEnemy = GameObject.FindGameObjectWithTag("Enemy");  //nicht verwendet
        //tower = GameObject.FindGameObjectWithTag("Tower");   //nicht verwendet
        
        nearestTower = FindNearestTower();   //zugehöriger Turm
        range = nearestTower.GetComponent<Tower>().range;
        shootSpeed = nearestTower.GetComponent<Tower>().bulletSpeed;
        damage = nearestTower.GetComponent<Tower>().damageAmount;

        UpdateTarget();

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, shootSpeed * Time.deltaTime);
        if(target == null)
        {
            UpdateTarget();
        }
       
        if(target == null)
        {
            Destroy(gameObject);
            return;
        }
        if(Vector3.Distance(transform.position, target.position) < 0.1f)
        {   
            damageScript.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        if(damageScript.isAlive == false)
        {
            Debug.Log("Destroyed");
            Destroy(gameObject);
        }
    }
    void UpdateTarget()
    {
        // Sucht nach allen Gegnern mit dem Tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        // Finde den nächsten Gegner innerhalb der Reichweite
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        // Wenn ein Gegner gefunden wurde, setze ihn als Ziel
        if (nearestEnemy != null && shortestDistance <= range)
        {
            Transform centerTransform = nearestEnemy.transform.Find("Center");

            target = centerTransform;
            damageScript = nearestEnemy.GetComponent<DamageTest>();
        }
        else
        {
            target = null; // Kein Gegner in Reichweite
        }
    }
    GameObject FindNearestTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower"); // Suche alle Türme
        float shortestDistance = Mathf.Infinity;
        GameObject nearestTower = null;

        // Finde den nächsten Turm
        foreach (GameObject tower in towers)
        {
            float distanceToTower = Vector3.Distance(transform.position, tower.transform.position);
            if (distanceToTower < shortestDistance)
            {
                shortestDistance = distanceToTower;
                nearestTower = tower;
            }
        }

        return nearestTower; // Gebe den nächstgelegenen Turm zurück
    }
}


