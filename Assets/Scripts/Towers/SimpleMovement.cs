using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public Vector3 moveDistance;
    public int speed = 2;
    public float start;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 targetPosition2;
    private bool targetTargeted = true;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        targetPosition = transform.position + (moveDistance / 2);
        targetPosition2 = transform.position - (moveDistance / 2);

        transform.position = targetPosition2 + (moveDistance * start);
    }

    // Update is called once per frame
    void Update()
    {
        if (targetTargeted)         //ping - pong 
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if(Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                targetTargeted = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition2, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition2) < 0.01f)
            {
                targetTargeted = true;
            }
        }
    }
    
}
