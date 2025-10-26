using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class CancelDictionaryProtoType : MonoBehaviour
{
    private Dictionary<Vector2Int, int> spawnGrid = new Dictionary<Vector2Int, int>();
    private Dictionary<Vector2Int, GameObject> gridVisuals = new Dictionary<Vector2Int, GameObject>();
    private List<Vector3> points3D = new List<Vector3>();

    public Material freeMat;
    public Material occupiedMat;
    public float yOffset = 0.05f;
    public GameObject plate;

    public int showDuration = 1000;

    private Terrain terrain;
    private bool IsTerrainInScene;

    private int tiling;
    private SpawnOnMouseClick spawnScript;
    private int maxDistance;

    public Material lineMaterial;     // Weisen wir im Inspector zu
    public float lineWidth = 0.1f;


    void Start()
    {
        spawnScript = FindObjectOfType<SpawnOnMouseClick>();
        tiling = spawnScript.tiling;
        maxDistance = tiling;

        terrain = FindObjectOfType<Terrain>();
        IsTerrainInScene = (terrain != null);
    }


    public void showCancelArea()
    {
        ShowOutlines(Outline(spawnGrid, maxDistance));
    }

    public void ShowOutlines(List<List<Vector2Int>> clusters)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Für jeden Cluster eine Linie erzeugen
        foreach (var cluster in clusters)
        {
            GameObject lineObj = new GameObject("Outline_Line");
            lineObj.transform.SetParent(transform);

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.loop = true; // schließt die Linie, damit sie einen Rand bildet
            lr.useWorldSpace = true;

            // Punkte umwandeln (Vector2Int → Vector3)
            Vector3[] positions = new Vector3[cluster.Count];
            for (int i = 0; i < cluster.Count; i++)
            {
                Vector2Int p = cluster[i];
                positions[i] = new Vector3(p.x, 0f, p.y); // Z = y für 2D-Grid im 3D-Raum
            }

            lr.positionCount = positions.Length;
            lr.SetPositions(positions);
        }
    }

    List<List<Vector2Int>> Outline(Dictionary<Vector2Int, int> grid, int cellSize)
    {
        // Schritt 1: Randpunkte finden
        List<Vector2Int> edgePoints = new List<Vector2Int>();
        foreach (var kvp in grid)
        {
            Vector2Int p = kvp.Key;
            if (kvp.Value != 1) continue;

            int neighborCount = 0;
            Vector2Int[] dirs = {
        new Vector2Int(cellSize, 0),
        new Vector2Int(-cellSize, 0),
        new Vector2Int(0, cellSize),
        new Vector2Int(0, -cellSize)
    };

            foreach (var d in dirs)
            {
                Vector2Int n = p + d;
                if (grid.ContainsKey(n) && grid[n] == 1)
                    neighborCount++;
            }

            if (neighborCount < 4)
                edgePoints.Add(p);
        }

        // Schritt 2: Cluster bilden
        List<List<Vector2Int>> clusters = new List<List<Vector2Int>>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        foreach (var start in edgePoints)
        {
            if (visited.Contains(start)) continue;

            List<Vector2Int> cluster = new List<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                cluster.Add(current);

                Vector2Int[] dirs = {
            new Vector2Int(cellSize, 0),
            new Vector2Int(-cellSize, 0),
            new Vector2Int(0, cellSize),
            new Vector2Int(0, -cellSize)
        };

                foreach (var d in dirs)
                {
                    Vector2Int next = current + d;
                    if (!visited.Contains(next) && edgePoints.Contains(next))
                    {
                        visited.Add(next);
                        queue.Enqueue(next);
                    }
                }
            }

            // Optional: sortiere Punkte im Uhrzeigersinn für saubere Mesh-Erstellung
            cluster.Sort((a, b) => Mathf.Atan2(a.y, a.x).CompareTo(Mathf.Atan2(b.y, b.x)));

            clusters.Add(cluster);
        }

        return clusters;
    }


    float Cross(Vector2 o, Vector2 a, Vector2 b)
    {
        return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
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
        float terrainHeight = Terrain.activeTerrain.SampleHeight(new Vector3(pos.x, 0, pos.y));
        points3D.Add(new Vector3(pos.x, terrainHeight, pos.y));
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
