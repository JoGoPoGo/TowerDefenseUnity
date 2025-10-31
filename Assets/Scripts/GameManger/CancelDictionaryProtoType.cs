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
        foreach (var cluster in clusters)
        {
            Debug.Log("Cluster" + cluster);
            foreach(Vector2Int c in cluster)
            {
                Debug.Log(c);
            }
        }

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

    

    public static List<List<Vector2Int>> Outline(Dictionary<Vector2Int, int> dic, int cellStep = 1, bool allowDiagonal = true)
    {
        // 1) Nachbarschafts-Offsets (konsistent verwenden)
        List<Vector2Int> neighOffsets4 = new List<Vector2Int> {
            new Vector2Int(cellStep, 0),
            new Vector2Int(-cellStep, 0),
            new Vector2Int(0, cellStep),
            new Vector2Int(0, -cellStep)
        };

        List<Vector2Int> neighOffsets8 = new List<Vector2Int>(neighOffsets4) {
            new Vector2Int(cellStep, cellStep),
            new Vector2Int(cellStep, -cellStep),
            new Vector2Int(-cellStep, cellStep),
            new Vector2Int(-cellStep, -cellStep)
        };

        var neighOffsets = allowDiagonal ? neighOffsets8 : neighOffsets4;

        // 2) Edge-Punkte finden (HashSet für schnellen Lookup)
        HashSet<Vector2Int> edgePoints = new HashSet<Vector2Int>();
        foreach (var kvp in dic)
        {
            if (kvp.Value != 1) continue;
            Vector2Int p = kvp.Key;

            int neighborCount = 0;
            foreach (var d in neighOffsets4) // Nur 4-Nachbarn für "Rand"-Definition (klassisch)
            {
                Vector2Int n = p + d;
                if (dic.ContainsKey(n) && dic[n] == 1) neighborCount++;
            }

            if (neighborCount < 4)
                edgePoints.Add(p);
        }

        // 3) Connected components (BFS) über die gleiche Nachbarschaftsdefinition (wichtig!)
        List<List<Vector2Int>> clusters = new List<List<Vector2Int>>();
        HashSet<Vector2Int> unvisited = new HashSet<Vector2Int>(edgePoints);

        while (unvisited.Count > 0)
        {
            // nimm irgendeinen Startpunkt aus unvisited
            Vector2Int start = default;
            foreach (var v in unvisited) { start = v; break; }

            Queue<Vector2Int> q = new Queue<Vector2Int>();
            List<Vector2Int> cluster = new List<Vector2Int>();

            q.Enqueue(start);
            unvisited.Remove(start);

            while (q.Count > 0)
            {
                Vector2Int cur = q.Dequeue();
                cluster.Add(cur);

                // Verwende dieselben Offsets wie beim edge-Finden (oder allowDiagonal für robustere Verbindung)
                foreach (var d in neighOffsets)
                {
                    Vector2Int nxt = cur + d;
                    if (unvisited.Contains(nxt))
                    {
                        unvisited.Remove(nxt);
                        q.Enqueue(nxt);
                    }
                }
            }

            clusters.Add(cluster);
        }

        for (int i = 0; i < clusters.Count; i++)
        {
            clusters[i] = SortCluster(clusters[i], cellStep, allowDiagonal);
        }
        // Optional: falls du eine geordnete Kontur brauchst, kannst du cluster weiter bearbeiten.
        // Hier geben wir komplette, zusammenhängende Edge-Point-Sets zurück (un-ordered lists).
        return clusters;
    }
    public static List<Vector2Int> SortCluster(List<Vector2Int> cluster, int step = 1, bool allowDiagonal = true)
    {
        if (cluster.Count <= 2) return cluster; // nichts zu sortieren

        // Set für schnellen Lookup
        HashSet<Vector2Int> points = new HashSet<Vector2Int>(cluster);
        List<Vector2Int> sorted = new List<Vector2Int>();

        // Startpunkt: der linkeste, dann niedrigste y (heuristik)
        Vector2Int current = cluster.OrderBy(p => p.x).ThenBy(p => p.y).First();
        sorted.Add(current);
        points.Remove(current);

        // Nachbarschaft definieren (im Uhrzeigersinn)
        List<Vector2Int> dirs = allowDiagonal
            ? new List<Vector2Int> {
            new Vector2Int(step, 0),
            new Vector2Int(step, step),
            new Vector2Int(0, step),
            new Vector2Int(-step, step),
            new Vector2Int(-step, 0),
            new Vector2Int(-step, -step),
            new Vector2Int(0, -step),
            new Vector2Int(step, -step)
            }
            : new List<Vector2Int> {
            new Vector2Int(step, 0),
            new Vector2Int(0, step),
            new Vector2Int(-step, 0),
            new Vector2Int(0, -step)
            };

        // Punkte sortieren, indem wir immer den nächsten Nachbarn entlang der Outline suchen
        while (points.Count > 0)
        {
            Vector2Int next = current;
            int minDist = int.MaxValue;

            // finde den nächsten Nachbarn (möglichst nah)
            foreach (var dir in dirs)
            {
                Vector2Int cand = current + dir;
                if (points.Contains(cand))
                {
                    next = cand;
                    minDist = 0;
                    break;
                }
            }

            // Falls kein direkter Nachbar (z. B. Lücke) → nächstgelegenen Punkt suchen
            if (minDist > 0)
            {
                next = points.OrderBy(p => (p - current).sqrMagnitude).First();
            }

            sorted.Add(next);
            points.Remove(next);
            current = next;
        }

        return sorted;
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
