using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{ 

    private DamageTest life;

    void Start()
    {
        life = GetComponent<DamageTest>();
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

