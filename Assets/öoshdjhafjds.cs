using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class öoshdjhafjds : MonoBehaviour
{
    // Start is called before the first frame update
    
    private bool sdkjöafkld = true;
    // Update is called once per frame
    void Start()
    {
        StartCoroutine(hochRunter());
    }
    private IEnumerator hochRunter()
    {
        while (true)
        {
            for (int i = 0; i < 100; i++)
                transform.position += new Vector3(0, 1, 0);
            for (int i = 0; i < 100; i++)
                transform.position -= new Vector3(0, 1, 0);
        }
    }
}
