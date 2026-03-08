using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DorOpen : MonoBehaviour
{
    public float moveRight = 2f;     // Wie weit die Tür nach rechts fährt
    public float speed = 2f;         // Bewegungsgeschwindigkeit
    public bool isLocked = false;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private bool isMoving = false;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + transform.right * moveRight;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && !isMoving && !isLocked) // Rechtsklick
        {
            if (isOpen)
                StartCoroutine(MoveDoor(closedPosition));
            else
                StartCoroutine(MoveDoor(openPosition));

            isOpen = !isOpen;
        }
    }

    System.Collections.IEnumerator MoveDoor(Vector3 target)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }
}
