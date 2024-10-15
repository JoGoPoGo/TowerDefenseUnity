using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThirdPersonMovementScript : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public float speed = 6f;

    public float mouseSensitivity = 100f;  // Neue Variable für Maus-Empfindlichkeit
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Mausbewegung für Kamera-/Spielerrotation einbinden
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; // Mausbewegung auf der X-Achse

        // Drehung des Spielers basierend auf der Mausbewegung
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle + mouseX, 0f); // Hinzufügen von Maussteuerung zur Drehung

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        // Animationen steuern
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            anim.SetFloat("Blend", 1.0f, 0.1f, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("Blend", 0.0f, 0.1f, Time.deltaTime);
        }

        // Sprinten, wenn LeftShift gedrückt wird
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 10.0f;
        }
        else
        {
            speed = 6.0f;
        }

        // Cursor entsperren, wenn Taste T gedrückt wird
        if (Input.GetKey(KeyCode.T))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // Cursor sperren/entsperren Methoden
    public void lockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void unlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
