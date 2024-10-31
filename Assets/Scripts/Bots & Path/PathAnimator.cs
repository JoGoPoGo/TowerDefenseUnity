using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;  // Für den Zugriff auf PathCreator

public class PrefabSpawnerAlongPath : MonoBehaviour
{
    public PathCreator pathCreator;    // Referenz zum PathCreator-Pfad
    public GameObject prefabToSpawn;   // Das Prefab, das gespawnt werden soll
    public float spawnInterval = 0.2f; // Abstand zwischen den Spawns in Einheiten
    public GameObject parent;

    void Start()
    {
        if (pathCreator != null && prefabToSpawn != null)
        {
            SpawnPrefabsAlongPath();
        }
    }

    void SpawnPrefabsAlongPath()
    {
        float distanceTravelled = 0f;
        float pathLength = pathCreator.path.length;

        // Schleife über den Pfad mit festgelegtem Abstand
        while (distanceTravelled < pathLength)
        {
            // Bestimme Position und Rotation entlang des Pfads
            Vector3 spawnPosition = pathCreator.path.GetPointAtDistance(distanceTravelled);
            Quaternion spawnRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled) * Quaternion.Euler(0, 0, 90);

            // Instanziere das Prefab
            Instantiate(prefabToSpawn, spawnPosition, spawnRotation, parent.transform);

            // Erhöhe den Abstand um das Intervall für den nächsten Spawn
            distanceTravelled += spawnInterval;
        }
    }
}

