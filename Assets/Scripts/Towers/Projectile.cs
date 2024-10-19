using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform target;
    public float shootSpeed = 5f;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, shootSpeed * Time.deltaTime);
        
        if(Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            Destroy(gameObject);
            return;
        }
    }
}
