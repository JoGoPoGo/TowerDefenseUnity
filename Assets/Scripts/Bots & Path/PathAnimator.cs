using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;  // Für den Zugriff auf PathCreator

public class PrefabSpawnerAlongPath : MonoBehaviour
{
    public PathCreator pathCreator;    // Referenz zum PathCreator-Pfad
    public GameObject prefabToSpawn;   // Das Prefab, das gespawnt werden soll
    public float spawnInterval = 0.2f; // Abstand zwischen den Spawns in Einheiten
    public GameObject parent;

    public int cancelRange = 2;

    private int tiling = 1;
    private CancelDictionaryProtoType dictionary;
    private GameObject gameManager;
    private SpawnOnMouseClick tilingScript;

    void Start()
    {
        if (pathCreator != null && prefabToSpawn != null)
        {
            SpawnPrefabsAlongPath();
        }

        tiling = 1;
        Debug.Log(tiling);
    }

    private void Awake()
    {
        tilingScript = FindObjectOfType<SpawnOnMouseClick>();
        tiling = tilingScript.tiling;
        dictionary = FindObjectOfType<CancelDictionaryProtoType>();
    }

    void SpawnPrefabsAlongPath()
    {
        float distanceTravelled = 0f;
        float pathLength = pathCreator.path.length;
        Vector3 lastCancelPosition = pathCreator.path.GetPointAtDistance(0);
        bool onThisPosCancel = false;

        // Schleife über den Pfad mit festgelegtem Abstand
        while (distanceTravelled < pathLength)
        {
            // Bestimme Position und Rotation entlang des Pfads
            Vector3 spawnPosition = pathCreator.path.GetPointAtDistance(distanceTravelled);
            
            Quaternion spawnRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled) * Quaternion.Euler(0, 0, 90);

            // Instanziere das Prefab
            Instantiate(prefabToSpawn, spawnPosition, spawnRotation, parent.transform);
            if(Vector3.Distance(spawnPosition, lastCancelPosition) > tiling)
            {
                onThisPosCancel = true;
                lastCancelPosition = spawnPosition;
            }
            else
            {
                onThisPosCancel = false;
            }
            if (onThisPosCancel)
            {
                CancelOn(spawnPosition);
            }
            // Erhöhe den Abstand um das Intervall für den nächsten Spawn
            distanceTravelled += spawnInterval;
        }
    }
    private void CancelOn(Vector3 cancelPosition)
    {
        // 1. Mitte des Rasters bestimmen
        Vector2Int center = new Vector2Int(
            Mathf.RoundToInt(cancelPosition.x / tiling) * tiling,
            Mathf.RoundToInt(cancelPosition.z / tiling) * tiling
        );

        float sqrRange = cancelRange * cancelRange;

        // 2. Iteriere über ein Quadrat um das Zentrum –
        //    prüfe aber Distanz, um wirklich einen Kreis zu bekommen
        for (int x = center.x - cancelRange; x <= center.x + cancelRange; x += tiling)
        {
            for (int y = center.y - cancelRange; y <= center.y + cancelRange; y += tiling)
            {
                Vector2Int p = new Vector2Int(x, y);

                // 3. Prüfe, ob der Punkt im Kreis liegt
                float sqrDist = (p.x - center.x) * (p.x - center.x)
                              + (p.y - center.y) * (p.y - center.y);

                if (sqrDist <= sqrRange)
                {
                    dictionary.OccupyPosition(p);
                }
            }
        }
    }

}

