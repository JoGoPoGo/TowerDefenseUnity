using PathCreation;
using UnityEngine;

public class MoveAlongPath : MonoBehaviour
{
    public PathCreator pathCreator; // Referenz zum PathCreator
    public float speed = 5f; // Geschwindigkeit der Bewegung
    private float distanceTravelled;

    void Update()
    {
        // Berechne die zurückgelegte Distanz
        distanceTravelled += speed * Time.deltaTime;

        // Setze die Position des Objekts entlang des Pfades
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
    }
}
