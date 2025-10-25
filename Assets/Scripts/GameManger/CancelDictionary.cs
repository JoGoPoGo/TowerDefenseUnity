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
    private List<Vector3> points3D = new List<Vector3>();

    public Material freeMat;
    public Material occupiedMat;
    public float cellSize = 1f;
    public float yOffset = 0.05f;
    public GameObject plate;

    public int showDuration = 1000;

    private Terrain terrain;
    private bool IsTerrainInScene;

    private int tiling;
    private SpawnOnMouseClick spawnScript;
    private int maxDistance;


    void Start()
    {
        spawnScript = FindObjectOfType <SpawnOnMouseClick>();
        tiling = spawnScript.tiling;
        maxDistance = tiling;

        terrain = FindObjectOfType<Terrain>();
        IsTerrainInScene = (terrain != null);

        
    }

    // --- Clusterbildung per Flood Fill ---


    public void showCancelArea()
    {
        checkDictionary();
        // Schritt 1: Punkte erfassen
        List<Vector2> points = new List<Vector2>();

        /*foreach (var cell in spawnGrid)
        {
            if (cell.Value == 1)
            {
                Vector2Int p = cell.Key;
                points.Add(new Vector2(p.x, p.y));
            }
        }

        // Schritt 2: Cluster bilden
        List<List<Vector2>> clusters = ClusterPoints(points, maxDistance); */

        List<List<Vector2>> clusters = BuildTightClusters(spawnGrid);

        // Schritt 3: Für jeden Cluster eine konvexe Hülle
        foreach (var cluster in clusters)
        {
            List<Vector2> hull = ComputeConvexHull(cluster);
            CreateCancelMesh(hull);

            // Dictionary für diesen Cluster aufbauen
            /*Dictionary<Vector2Int, int> clusterGrid = new Dictionary<Vector2Int, int>();          //von hier
            foreach (var p in cluster)
            {
                Vector2Int pi = new Vector2Int((int)p.x, (int)p.y);
                if (spawnGrid.ContainsKey(pi))
                    clusterGrid[pi] = spawnGrid[pi];
            }                                                                                           //bis hier

            // Mesh nur für die gültigen Rasterzellen erzeugen
            CreateCancelMeshFromGrid(clusterGrid);  */


            // Schritt 4: Debug: Linien zeichnen
            for (int i = 0; i < hull.Count; i++)
            {
                Vector2 a = hull[i];
                Vector2 b = hull[(i + 1) % hull.Count];

                float ha = IsTerrainInScene ? terrain.SampleHeight(new Vector3(a.x, 0, a.y)) : 0f;
                float hb = IsTerrainInScene ? terrain.SampleHeight(new Vector3(b.x, 0, b.y)) : 0f;

                GameObject lineObj = new GameObject("HullLine");
                LineRenderer lr = lineObj.AddComponent<LineRenderer>();

                lr.positionCount = 2;
                lr.SetPosition(0, new Vector3(a.x, ha + yOffset, a.y));
                lr.SetPosition(1, new Vector3(b.x, hb + yOffset, b.y));

                lr.startWidth = 0.05f;
                lr.endWidth = 0.05f;
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = Color.red;
                lr.endColor = Color.red;
            }
        }

    }

    List<List<Vector2>> BuildTightClusters(Dictionary<Vector2Int, int> grid)
    {
        List<List<Vector2>> clusters = new List<List<Vector2>>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        // Alle belegten Punkte durchlaufen
        foreach (var kvp in grid)
        {
            Vector2Int start = kvp.Key;
            if (kvp.Value != 1 || visited.Contains(start))
                continue;

            List<Vector2> cluster = new List<Vector2>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                cluster.Add(new Vector2(current.x, current.y));

                // Nur orthogonale Nachbarn (oben, unten, links, rechts)
                Vector2Int[] neighbors = new Vector2Int[]
                {
                new Vector2Int(current.x + 1, current.y),
                new Vector2Int(current.x - 1, current.y),
                new Vector2Int(current.x, current.y + 1),
                new Vector2Int(current.x, current.y - 1)
                };

                foreach (var n in neighbors)
                {
                    if (visited.Contains(n)) continue;
                    if (grid.ContainsKey(n) && grid[n] == 1)
                    {
                        visited.Add(n);
                        queue.Enqueue(n);
                    }
                }
            }

            clusters.Add(cluster);
        }

        return clusters;
    }


    void CreateCancelMesh(List<Vector2> hull)
    {
        if (hull.Count < 3) return;

        hull.Reverse(); 

        // 1️⃣ Vertices erzeugen
        Vector3[] vertices = new Vector3[hull.Count];
        for (int i = 0; i < hull.Count; i++)
        {
            float h = IsTerrainInScene ? terrain.SampleHeight(new Vector3(hull[i].x, 0, hull[i].y)) : 0f;
            vertices[i] = new Vector3(hull[i].x, h + yOffset, hull[i].y);
        }

        // 2️⃣ Triangulation (für konvexe Polygone einfaches Fan-Muster)
        List<int> triangles = new List<int>();
        for (int i = 1; i < hull.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        // 3️⃣ Mesh erstellen
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // 4️⃣ GameObject mit MeshRenderer erzeugen
        GameObject area = new GameObject("CancelAreaMesh", typeof(MeshFilter), typeof(MeshRenderer));
        area.GetComponent<MeshFilter>().mesh = mesh;

        // 5️⃣ Material setzen
        var renderer = area.GetComponent<MeshRenderer>();
        renderer.material = occupiedMat;

        // Optional: leichtes Offset, damit sie nicht flackert mit Terrain
        area.transform.position += Vector3.up * 0.01f;

        // Optional: Hierarchie aufräumen
        area.transform.parent = transform;
    }
    void CreateCancelMeshFromGrid(Dictionary<Vector2Int, int> grid)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        foreach (var kvp in grid)
        {
            Vector2Int p = kvp.Key;
            if (kvp.Value != 1) continue;

            // Nachbarn prüfen (für ein Quad)
            Vector2Int right = new Vector2Int(p.x + maxDistance, p.y);
            Vector2Int up = new Vector2Int(p.x, p.y + maxDistance);
            Vector2Int upRight = new Vector2Int(p.x + maxDistance, p.y + maxDistance);

            if (grid.ContainsKey(right) && grid.ContainsKey(up) && grid.ContainsKey(upRight) &&
                grid[right] == 1 && grid[up] == 1 && grid[upRight] == 1)
            {
                // Terrainhöhe pro Eckpunkt abfragen
                float h1 = IsTerrainInScene ? terrain.SampleHeight(new Vector3(p.x, 0, p.y)) : 0f;
                float h2 = IsTerrainInScene ? terrain.SampleHeight(new Vector3(right.x, 0, right.y)) : 0f;
                float h3 = IsTerrainInScene ? terrain.SampleHeight(new Vector3(upRight.x, 0, upRight.y)) : 0f;
                float h4 = IsTerrainInScene ? terrain.SampleHeight(new Vector3(up.x, 0, up.y)) : 0f;

                int baseIndex = vertices.Count;

                vertices.Add(new Vector3(p.x, h1 + yOffset, p.y));
                vertices.Add(new Vector3(right.x, h2 + yOffset, right.y));
                vertices.Add(new Vector3(upRight.x, h3 + yOffset, upRight.y));
                vertices.Add(new Vector3(up.x, h4 + yOffset, up.y));

                // Zwei Dreiecke pro Quad
                // Zwei Dreiecke pro Quad (gegen den Uhrzeigersinn)
                triangles.Add(baseIndex);      // unten links
                triangles.Add(baseIndex + 2);  // oben rechts
                triangles.Add(baseIndex + 1);  // unten rechts

                triangles.Add(baseIndex);      // unten links
                triangles.Add(baseIndex + 3);  // oben links
                triangles.Add(baseIndex + 2);  // oben rechts

            }
        }

        if (vertices.Count < 3)
            return;

        // Mesh erstellen
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // GameObject mit MeshFilter & Renderer
        GameObject area = new GameObject("CancelAreaMesh", typeof(MeshFilter), typeof(MeshRenderer));
        area.GetComponent<MeshFilter>().mesh = mesh;

        var renderer = area.GetComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(1f, 0f, 0f, 0.35f);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.renderQueue = 3000;
        renderer.material = mat;

        area.transform.parent = transform;
    }


    List<List<Vector2>> ClusterPoints(List<Vector2> points, float maxDist)
    {
        List<List<Vector2>> clusters = new List<List<Vector2>>();
        HashSet<Vector2> visited = new HashSet<Vector2>();

        foreach (var p in points)
        {
            if (visited.Contains(p))
                continue;

            List<Vector2> cluster = new List<Vector2>();
            Queue<Vector2> queue = new Queue<Vector2>();
            queue.Enqueue(p);
            visited.Add(p);

            while (queue.Count > 0)
            {
                Vector2 current = queue.Dequeue();
                cluster.Add(current);

                foreach (var other in points)
                {
                    if (visited.Contains(other))
                        continue;

                    if (Vector2.Distance(current, other) <= maxDist)
                    {
                        visited.Add(other);
                        queue.Enqueue(other);
                    }
                }
            }

            clusters.Add(cluster);
        }

        return clusters;
    }

    void checkDictionary()
    {
        foreach(KeyValuePair<Vector2Int,int> pair in spawnGrid)
        {
            Debug.Log(pair.Key);
        }
    }

    // --- Konvexe Hülle (wie zuvor) ---
    List<Vector2> ComputeConvexHull(List<Vector2> points)
    {
        if (points.Count <= 3)
            return new List<Vector2>(points);

        points.Sort((a, b) => a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));
        List<Vector2> lower = new List<Vector2>();
        foreach (var p in points)
        {
            while (lower.Count >= 2 && Cross(lower[lower.Count - 2], lower[lower.Count - 1], p) <= 0)
                lower.RemoveAt(lower.Count - 1);
            lower.Add(p);
        }

        List<Vector2> upper = new List<Vector2>();
        for (int i = points.Count - 1; i >= 0; i--)
        {
            Vector2 p = points[i];
            while (upper.Count >= 2 && Cross(upper[upper.Count - 2], upper[upper.Count - 1], p) <= 0)
                upper.RemoveAt(upper.Count - 1);
            upper.Add(p);
        }

        lower.RemoveAt(lower.Count - 1);
        upper.RemoveAt(upper.Count - 1);
        lower.AddRange(upper);
        return lower;
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
