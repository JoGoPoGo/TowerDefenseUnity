using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public Transform target; //um was soll sich die Kamera drehen? -- siehe Public
    public float distance = 10f; //Abstand zum Target -- siehe Public
    public float RotationSpeed = 50f; //Geschwindigkeit -- siehe Public
    public float Height = 10f; //Höhe der Kamera -- siehe Public

    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(0, Height, -distance);  //berechnet Standpunkt
    }

    // Update is called once per frame
    void Update()
    {
        offset = Quaternion.AngleAxis(RotationSpeed * Time.deltaTime, Vector3.up) * offset;
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}
