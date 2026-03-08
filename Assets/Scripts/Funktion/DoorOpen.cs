using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public float moveRight = 2f;     // Wie weit die Tür nach rechts fährt
    public float speed = 2f;         // Bewegungsgeschwindigkeit
    public bool isLocked = false;
    public int lvlNumber = 0;

    public TMP_Text statusText;
    public GameObject licht;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private bool isMoving = false;

    private bool firstL = true;
    private bool firstUL = true;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + transform.right * moveRight;
    }

    private void Update()
    {
        if (isLocked && firstL)
        {
            Lock();
            firstL = false;
            firstUL = true;
        }
        if(firstUL && !isLocked)
        {
            Unlock();
            firstUL = false;
            firstL = true;
        }
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
    private void Lock()
    {
        if(statusText != null)
        statusText.text = "<color=#FF0000>LOCKED</color>";
        if(licht != null)
        licht.SetActive(false);
    }

    private void Unlock()
    {
        if(statusText != null)
        statusText.text = "<color=#00FF00>UNLOCKED</color>";
        if(licht != null)
        licht.SetActive(true);
    }
}
