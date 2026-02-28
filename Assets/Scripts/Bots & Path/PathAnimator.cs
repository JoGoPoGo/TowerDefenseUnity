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
    public LayerMask bridgeLayer;

    public int cancelRange = 2;

    public bool RandomBool = false;
    private int tiling = 1;
    private CancelDictionaryProtoType dictionary;
    private GameObject gameManager;
    private SpawnOnMouseClick tilingScript;

    void Start()
    {  
        SnapPathToTerrain();
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SnapPathToTerrain();
        }
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
            Terrain terrain = Terrain.activeTerrain;
            if (terrain == null)
            {
                Debug.Log("FEHLER: Es wurde kein aktives Terrain in der Szene gefunden!");
                break;
            }
            TerrainData data = terrain.terrainData;

            // Position relativ zum Terrain
            Vector3 terrainPos = spawnPosition - terrain.transform.position;

            float normX = terrainPos.x / data.size.x;
            float normZ = terrainPos.z / data.size.z;

            // Normale der Terrain-Oberfläche
            Vector3 terrainNormal = data.GetInterpolatedNormal(normX, normZ);
            //Quaternion spawnRotation = pathCreator.path.GetRotationAtDistance(distanceTravelled) * Quaternion.Euler(0, 0, 90);

            Vector3 forward = pathCreator.path.GetDirectionAtDistance(distanceTravelled);

            // Rotation mit Terrain-Neigung
            Quaternion spawnRotation = Quaternion.LookRotation(forward, terrainNormal);

            if (RandomBool)
            {
                spawnPosition += new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
                Vector3 euler = spawnRotation.eulerAngles;
                euler += new Vector3(0, Random.Range(-90, 90), 0);
                spawnRotation = Quaternion.Euler(euler);
            }

            // Instanziere das Prefab
            Instantiate(prefabToSpawn, spawnPosition, spawnRotation, parent.transform);
            // Passe das Terrain an
            SetTerrainCircleHeight(spawnPosition, 2f, spawnPosition.y - 0.05f);


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
    void SnapPathToTerrain()
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
            return;        
        BezierPath bezierPath = pathCreator.bezierPath;

        for (int i = 0; i < bezierPath.NumPoints; i++)
        {
            Vector3 point = bezierPath.GetPoint(i);

            RaycastHit bridgeHit;
            bool isOnBridge = Physics.Raycast(
                point,
                Vector3.up,
                out bridgeHit,
                500f,
                bridgeLayer
            );
            if (isOnBridge)
            {
                Debug.Log("Bridge Hittet");
                point.y = bridgeHit.point.y;
            }
            else
            {
                float terrainHeight = terrain.SampleHeight(point)
                                  + terrain.transform.position.y;

                point.y = terrainHeight;
            }
            bezierPath.SetPoint(i, point);
            //Debug.Log(point);
        }
    }
    void SetTerrainCircleHeight(Vector3 worldPos, float radius, float targetWorldHeight)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return;

        TerrainData data = terrain.terrainData;

        int resolution = data.heightmapResolution;

        // Position relativ zum Terrain
        Vector3 terrainPos = worldPos - terrain.transform.position;

        int centerX = Mathf.RoundToInt((terrainPos.x / data.size.x) * resolution);
        int centerZ = Mathf.RoundToInt((terrainPos.z / data.size.z) * resolution);

        int radiusInSamples = Mathf.RoundToInt((radius / data.size.x) * resolution);

        int size = radiusInSamples * 2;

        // Clamp gegen Randfehler
        int startX = Mathf.Clamp(centerX - radiusInSamples, 0, resolution - 1);
        int startZ = Mathf.Clamp(centerZ - radiusInSamples, 0, resolution - 1);

        size = Mathf.Clamp(size, 0, resolution - startX);
        size = Mathf.Clamp(size, 0, resolution - startZ);

        float[,] heights = data.GetHeights(startX, startZ, size, size);

        // Zielhöhe normalisieren (0–1)
        float normalizedHeight =
            (targetWorldHeight - terrain.transform.position.y) / data.size.y;

        normalizedHeight = Mathf.Clamp01(normalizedHeight);

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float dist = Vector2.Distance(
                    new Vector2(x, z),
                    new Vector2(radiusInSamples, radiusInSamples));

                if (dist < radiusInSamples)
                {
                    heights[z, x] = normalizedHeight;
                }
            }
        }

        data.SetHeights(startX, startZ, heights);
    }
}

