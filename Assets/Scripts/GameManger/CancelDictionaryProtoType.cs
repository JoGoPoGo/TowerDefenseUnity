using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class CancelDictionaryProtoType : MonoBehaviour
{
    private Dictionary<Vector2Int, int> spawnGrid = new Dictionary<Vector2Int, int>();

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
    public int smootheness;
    public bool active = false;

    void Start()
    {
        spawnScript = FindObjectOfType<SpawnOnMouseClick>();
        tiling = spawnScript.tiling;
        maxDistance = tiling;

        terrain = FindObjectOfType<Terrain>();
        IsTerrainInScene = (terrain != null);
    }

    public void Buttonpress()
    {
        if (active)
        {
            HideCancelArea();
        }

        else
        {
            ShowCancelArea();
        }
    }

    public void ShowCancelArea()
    {
        active = true;
        ShowOutlines(Outline(spawnGrid, maxDistance));
        
    }
    public void HideCancelArea()
    {
        active = false;
        // Alle Child-GameObjects (Outline-Lines) entfernen
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void ShowOutlines(List<List<Vector2Int>> clusters)
    {
        /*foreach (var cluster in clusters)        //Debug
        {
            Debug.Log("Cluster" + cluster);
            foreach(Vector2Int c in cluster)
            {
                Debug.Log(c);
            }
        }*/

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
            lr.sharedMaterial = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.loop = true; // schließt die Linie, damit sie einen Rand bildet
            lr.useWorldSpace = true;

            var simplified = RemoveCollinear(cluster);
            simplified = ReduceEveryNth(simplified, 3);

            // Punkte umwandeln (Vector2Int → Vector3)
            List<Vector3> basePoints = new List<Vector3>();

            Terrain terrain = IsTerrainInScene ? Terrain.activeTerrain : null;

            foreach (var p in simplified)
            {
                float height = yOffset;

                if (terrain != null)
                    height += terrain.SampleHeight(new Vector3(p.x, 0, p.y));

                basePoints.Add(new Vector3(p.x, height, p.y));
            }

            List<Vector3> bezierPoints = CreateBezierPath(
                basePoints,
                samplesPerSegment: 10,
                smoothness: 0.15f
            );

            lr.positionCount = bezierPoints.Count;
            lr.SetPositions(bezierPoints.ToArray());

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

        return clusters;
    }
    public static List<Vector2Int> SortCluster(List<Vector2Int> cluster, int step = 1, bool allowDiagonal = true)
    {
        if (cluster.Count <= 2) return cluster; // nichts zu sortieren

        // Set für schnellen Lookup
        HashSet<Vector2Int> points = new HashSet<Vector2Int>(cluster);
        List<Vector2Int> sorted = new List<Vector2Int>();

        // Startpunkt: der linkeste, dann niedrigste y (heuristik)
        Vector2Int current = cluster[0];
        for (int i = 1; i < cluster.Count; i++)
        {
            var p = cluster[i];
            if (p.x < current.x || (p.x == current.x && p.y < current.y))
                current = p;
        }
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

    public void ShowArea(List<List<Vector2Int>> clusters)
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        foreach (var cluster in clusters)
        {
            GameObject areaObj = new GameObject("Area_Fill");
            areaObj.transform.SetParent(transform);

            MeshFilter mf = areaObj.AddComponent<MeshFilter>();
            MeshRenderer mr = areaObj.AddComponent<MeshRenderer>();

            mr.material = new Material(Shader.Find("Standard"));
            mr.material.color = new Color(0f, 1f, 0f, 0.3f);


            Mesh mesh = CreateMeshFromPoints(cluster);
            mf.mesh = mesh;
        }
    }

    // ---- Hilfsfunktion: Punktwolke in Mesh umwandeln ----
    private Mesh CreateMeshFromPoints(List<Vector2Int> points)
    {
        // Sortiere Punkte im Uhrzeigersinn
        var ordered = points.OrderBy(p => Mathf.Atan2(p.y - points[0].y, p.x - points[0].x)).ToList();

        // 2D-Vektoren in 3D umwandeln
        Vector3[] verts = ordered.Select(p =>
        {
            float height = yOffset;
            if (IsTerrainInScene)
                height += Terrain.activeTerrain.SampleHeight(new Vector3(p.x, 0, p.y));

            return new Vector3(p.x, height, p.y);
        }).ToArray();


        // Triangulation (Unity hat keine eingebaute einfache Methode -> einfache Lib oder Fan-Triangulation)
        List<int> tris = new List<int>();
        for (int i = 1; i < verts.Length - 1; i++)
        {
            tris.Add(0);
            tris.Add(i);
            tris.Add(i + 1);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
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

    // Kubische Bezier-Interpolation (Für Bezier-Path)
    static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1f - t;
        return
            u * u * u * p0 +
            3f * u * u * t * p1 +
            3f * u * t * t * p2 +
            t * t * t * p3;
    }
    
    //Bezier-Pfad aus Outline-Punkten erzeugen
    static List<Vector3> CreateBezierPath(List<Vector3> points,int samplesPerSegment = 8,float smoothness = 0.25f)
    {
        List<Vector3> result = new List<Vector3>();
        int count = points.Count;

        for (int i = 0; i < count; i++)
        {
            Vector3 p0 = points[i];
            Vector3 pPrev = points[(i - 1 + count) % count];
            Vector3 pNext = points[(i + 1) % count];

            // Tangente bestimmen
            Vector3 dir = (pNext - pPrev) * smoothness;

            Vector3 c1 = p0 + dir;
            Vector3 c2 = pNext - dir;

            for (int s = 0; s < samplesPerSegment; s++)
            {
                float t = s / (float)samplesPerSegment;
                result.Add(Bezier(p0, c1, c2, pNext, t));
            }
        }

        return result;
    }

    //Jeden n-ten Punkt behalten
    static List<Vector2Int> ReduceEveryNth(List<Vector2Int> points, int step = 2)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        for (int i = 0; i < points.Count; i += step)
            result.Add(points[i]);

        return result;
    }

    //nur Richtungswechsel behalten
    static List<Vector2Int> RemoveCollinear(List<Vector2Int> points)
    {
        if (points.Count < 3) return points;

        List<Vector2Int> result = new List<Vector2Int>();

        for (int i = 0; i < points.Count; i++)
        {
            Vector2Int prev = points[(i - 1 + points.Count) % points.Count];
            Vector2Int curr = points[i];
            Vector2Int next = points[(i + 1) % points.Count];

            Vector2Int d1 = curr - prev;
            Vector2Int d2 = next - curr;

            if (d1.x * d2.y - d1.y * d2.x != 0)
                result.Add(curr); // Richtungswechsel
        }

        return result;
    }

}
