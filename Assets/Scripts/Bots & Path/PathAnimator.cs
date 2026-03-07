using PathCreation;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;  // Für den Zugriff auf PathCreator
using Unity.VisualScripting;
using UnityEngine;


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

        while (distanceTravelled < pathLength)
        {
            // =============================
            // 1?. Position entlang des Pfads
            // =============================
            Vector3 spawnPosition = pathCreator.path.GetPointAtDistance(distanceTravelled);

            // =============================
            // 2?. Brücken-Check
            // =============================
            RaycastHit bridgeHit;
            bool isOnBridge = Physics.Raycast(
                spawnPosition + Vector3.up * 10f,
                Vector3.down,
                out bridgeHit,
                20f,
                bridgeLayer
            );

            if (isOnBridge)
            {
                distanceTravelled += spawnInterval;
                continue;        //überspringt den rest des Codes dieser Iteration und beginnt mit der nächsten dieser While Schleife
            }

            // =============================
            // 3?. Terrain holen
            // =============================
            Terrain terrain = Terrain.activeTerrain;
            if (terrain == null)
            {
                Debug.Log("FEHLER: Es wurde kein aktives Terrain in der Szene gefunden!");
                break;
            }

            TerrainData data = terrain.terrainData;

            // =============================
            // 4?. Optionale Randomisierung der Position
            // =============================
            if (RandomBool)
            {
                float randomOffsetX = Mathf.Clamp(RandomGaussian(0f, 0.4f), -1f, 1f);
                float randomOffsetZ = Mathf.Clamp(RandomGaussian(0f, 0.4f), -1f, 1f);

                spawnPosition += new Vector3(randomOffsetX, 0, randomOffsetZ);
            }

            // =============================
            // 5?. Terrain-Daten an der FINALEN Position holen
            // =============================
            Vector3 terrainPos = spawnPosition - terrain.transform.position;

            float normX = terrainPos.x / data.size.x;
            float normZ = terrainPos.z / data.size.z;

            Vector3 terrainNormal = data.GetInterpolatedNormal(normX, normZ);

            // Richtung entlang des Pfades
            Vector3 forward = pathCreator.path.GetDirectionAtDistance(distanceTravelled);

            // =============================
            // 7?. Terrain anpassen
            // =============================
            /*SetTerrainCircleHeightSmooth(spawnPosition, 2f, 2f, spawnPosition.y*/

            // Rotation an Terrainneigung anpassen
            Quaternion spawnRotation = Quaternion.LookRotation(forward, terrainNormal);

            // =============================
            // 6?. Optionale Random Y Rotation
            // =============================
            if (RandomBool)
            {
                Vector3 euler = spawnRotation.eulerAngles;
                euler.y += Random.Range(-90f, 90f);
                spawnRotation = Quaternion.Euler(euler);
            }

            // =============================
            // 6?. Prefab instanziieren
            // =============================
            Instantiate(prefabToSpawn, spawnPosition, spawnRotation, parent.transform);

            // =============================
            // 8?. Cancel-System
            // =============================
            if (Vector3.Distance(spawnPosition, lastCancelPosition) > tiling)
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

            // =============================
            // 9?. Weiter entlang des Pfads
            // =============================
            distanceTravelled += spawnInterval;
        }
    }
    float RandomGaussian(float mean, float stdDev)
    {
        float u1 = 1.0f - Random.value;
        float u2 = 1.0f - Random.value;
        float randStdNormal =
            Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
            Mathf.Sin(2.0f * Mathf.PI * u2);

        return mean + stdDev * randStdNormal;
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
        Debug.Log("SnapPathToTerrain");
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
            return;        
        BezierPath bezierPath = pathCreator.bezierPath;

        for (int i = 0; i < bezierPath.NumPoints; i++)
        {
            Vector3 point = bezierPath.GetPoint(i);
            //Debug.Log(i  + ": " + point);

            RaycastHit bridgeHit;
            bool isOnBridge = Physics.Raycast(
                point + Vector3.up*200f,
                Vector3.down,
                out bridgeHit,
                500f,
                bridgeLayer
            );

            //Debug.Log(bridgeHit.distance);
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
    void SetTerrainCircleHeightSmooth(Vector3 worldPos, float radius, float blend, float targetWorldHeight)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return;

        TerrainData data = terrain.terrainData;
        int resolution = data.heightmapResolution;

        Vector3 terrainPos = worldPos - terrain.transform.position;

        int centerX = Mathf.RoundToInt((terrainPos.x / data.size.x) * resolution);
        int centerZ = Mathf.RoundToInt((terrainPos.z / data.size.z) * resolution);

        int radiusSamples = Mathf.RoundToInt((radius / data.size.x) * resolution);
        int blendSamples = Mathf.RoundToInt((blend / data.size.x) * resolution);

        int totalRadius = radiusSamples + blendSamples;

        int startX = Mathf.Clamp(centerX - totalRadius, 0, resolution - 1);
        int startZ = Mathf.Clamp(centerZ - totalRadius, 0, resolution - 1);

        int size = totalRadius * 2;

        size = Mathf.Clamp(size, 0, resolution - startX);
        size = Mathf.Clamp(size, 0, resolution - startZ);

        float[,] heights = data.GetHeights(startX, startZ, size, size);

        float normalizedHeight =
            (targetWorldHeight - terrain.transform.position.y) / data.size.y;

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float dist = Vector2.Distance(
                    new Vector2(x, z),
                    new Vector2(totalRadius, totalRadius)
                );

                if (dist < radiusSamples)
                {
                   heights[z, x] = normalizedHeight;
                }
                else if (dist < totalRadius)
                {
                    float t = (dist - radiusSamples) / blendSamples;
                    t = Mathf.SmoothStep(0, 1, t);

                    heights[z, x] = Mathf.Lerp(
                        normalizedHeight,
                        heights[z, x],
                        t
                    );
                }
            }
        }

        data.SetHeights(startX, startZ, heights);
    }
}


