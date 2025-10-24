using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class CancelDictionary : MonoBehaviour
{
    // Key = Gitterposition, Value = 0 (frei) oder 1 (besetzt)
    private Dictionary<Vector2Int, int> spawnGrid = new Dictionary<Vector2Int, int>();
    private Dictionary<Vector2Int, GameObject> gridVisuals = new Dictionary<Vector2Int, GameObject>();

    public Material freeMat;
    public Material occupiedMat;
    public float cellSize = 1f;
    public float yOffset = 0.05f;
    public GameObject plate;

    private Terrain terrain;
    private bool IsTerrainInScene;

    private int tiling;
    private SpawnOnMouseClick spawnScript;


    void Start()
    {
        spawnScript = FindObjectOfType <SpawnOnMouseClick>();
        tiling = spawnScript.tiling;

        terrain = FindObjectOfType<Terrain>();
        IsTerrainInScene = (terrain != null);
    }

    public void showCancelArea()
    {
        

        /*foreach (KeyValuePair<Vector2Int, int> cell in spawnGrid)
        {
            Vector2Int pos = cell.Key;
            int value = cell.Value;

            if (IsTerrainInScene && value == 1)
            {
                Terrain terrain = Terrain.activeTerrain;
                Vector3 terrainPos = terrain.transform.position;
                Vector3 relativePos = new Vector3(pos.x - terrainPos.x, pos.y - terrainPos.z);

                float normalizedX = relativePos.x / terrain.terrainData.size.x;
                float normalizedZ = relativePos.z / terrain.terrainData.size.z;

                float terrainHeight = Terrain.activeTerrain.SampleHeight(new Vector3(pos.x, 0, pos.y));
                Vector3 normal = terrain.terrainData.GetInterpolatedNormal(normalizedX, normalizedZ);
                Instantiate(plate, new Vector3(pos.x, terrainHeight + yOffset, pos.y), Quaternion.FromToRotation(Vector3.up, normal));
            }
            else
            {
                if(value == 1)
                {
                    GameObject plate1 = Instantiate(plate, new Vector3(pos.x, yOffset, pos.y), Quaternion.identity);
                    PlateScale(4, plate1);

                }
            }


        }
        foreach (KeyValuePair<Vector2Int, int> cell in spawnGrid)
        {
            Vector3 pos = new Vector3(cell.Key.x * cellSize, yOffset, cell.Key.y * cellSize);
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);

            quad.transform.position = pos;
            quad.transform.rotation = Quaternion.Euler(90, 0, 0); // nach oben zeigend
            quad.transform.localScale = new Vector3(cellSize, cellSize, 1);

            // Material wählen
            MeshRenderer rend = quad.GetComponent<MeshRenderer>();
            rend.material = (cell.Value == 0) ? freeMat : occupiedMat;

            // Optional: Hierarchiestruktur aufräumen
            quad.transform.parent = transform;

            gridVisuals[cell.Key] = quad;
        }*/
    }


    // Prüfen, ob an einer Position gespawnt werden darf
    public bool CanSpawnAt(Vector2Int pos)
    {
        return !spawnGrid.ContainsKey(pos) || spawnGrid[pos] == 0;
    }

    // Platz als besetzt markieren
    public void OccupyPosition(Vector2Int pos)
    {
        spawnGrid[pos] = 1;
    }

    // Platz wieder freigeben
    public void FreePosition(Vector2Int pos)
    {
        spawnGrid[pos] = 0;
    }

    private void PlateScale(int scale, GameObject plate1)
    {
        plate1.transform.localScale = new Vector3(scale, 1, scale);
    }
}
