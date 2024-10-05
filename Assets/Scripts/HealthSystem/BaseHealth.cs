using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    public float health;
    public Transform enemy1;
    public Transform enemy2;
    public Transform enemy3;

    public int damage;
    public float detectionRange = 1f; //Wie weit entfernt erhält die Base schaden? --siehe public


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckEnemyProximity(enemy1);
        CheckEnemyProximity(enemy2);
        CheckEnemyProximity(enemy3);
    }

    void CheckEnemyProximity(Transform enemy)
    {
        if(Vector3.Distance(transform.position, enemy.position) < detectionRange)
        {
            TakeDamage(damage);
        }
    }

    void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
