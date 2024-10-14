using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScipt : MonoBehaviour
{
    private BaseHealth basehealth;
    // Start is called before the first frame update
    void Start()
    {
        basehealth = GameObject.Find("Base2").GetComponent<BaseHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            basehealth.TakeDamage(30);
        }
    }
}
