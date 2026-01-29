using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{ 

    private EnemyScript life;

    void Start()
    {
        life = GetComponent<EnemyScript>();
    }

    void Update()
    {
        if (life.currentHealth <= 0)
        {
            Destroy(gameObject);
            return;
        }
    }
}

