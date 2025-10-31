using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        spawnScript = FindObjectOfType<SpawnOnMouseClick>();
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

    List<List<Vector2Int>> Outline(Dictionary<Vector2Int, int> dic)
    {
        List<Vector2Int> unsorted = new List<Vector2Int>();

        foreach (KeyValuePair<Vector2Int, int> pair in dic)
        {
            Vector2Int point = pair.Key;
            if (pair.Value == 1)
            {
                List<Vector2Int> proofList = new List<Vector2Int>();

                Vector2Int yPlus = new Vector2Int(point.x, point.y + maxDistance);
                if (dic.ContainsKey(yPlus))
                    proofList.Add(yPlus);
                Vector2Int yMinus = new Vector2Int(point.x, point.y - maxDistance);
                if (dic.ContainsKey(yMinus))
                    proofList.Add(yMinus);
                Vector2Int xPlus = new Vector2Int(point.x + maxDistance, point.y);
                if (dic.ContainsKey(xPlus))
                    proofList.Add(xPlus);
                Vector2Int xMinus = new Vector2Int(point.x - maxDistance, point.y);
                if (dic.ContainsKey(xMinus))
                    proofList.Add(xMinus);

                if (proofList.Count < 4)
                {
                    unsorted.Add(point);
                }
            }
        }

        List<List<Vector2Int>> clusters = new List<List<Vector2Int>>();

        foreach (var point in unsorted)
        {
            List<Vector2Int> cluster = new List<Vector2Int>();

            Vector2Int transferPoint = point;

            while (unsorted.Contains(transferPoint))
            {
                Vector2Int yPlus = new Vector2Int(transferPoint.x, transferPoint.y + maxDistance);
                Vector2Int yMinus = new Vector2Int(transferPoint.x, transferPoint.y - maxDistance);
                Vector2Int xPlus = new Vector2Int(transferPoint.x + maxDistance, transferPoint.y);
                Vector2Int xMinus = new Vector2Int(transferPoint.x - maxDistance, transferPoint.y);

                Vector2Int next = transferPoint;

                if (unsorted.Contains(yPlus))
                {
                    next = yPlus;
                    cluster.Add(transferPoint);
                }
                else if (unsorted.Contains(xPlus))
                {
                    next = xPlus;
                    cluster.Add(transferPoint);
                }
                else if (unsorted.Contains(yMinus))
                {
                    next = yMinus;
                    cluster.Add(transferPoint);
                }
                else if (unsorted.Contains(xMinus))
                {
                    next = xMinus;
                    cluster.Add(transferPoint);
                }

                unsorted.Remove(transferPoint);

                transferPoint = next;
            }
            sort(cluster);
            clusters.Add(cluster);
        }
        return clusters;
    }

    List<Vector2Int> sort(List<Vector2Int> list)
    {
        return list;
    }

    List<List<Vector2Int>> BuildTightClustersPro(Dictionary<Vector2Int, int> grid)
    {
        List<Vector2Int> edgePoints = new List<Vector2Int>();
        foreach (var kvp in grid)
        {
            Vector2Int p = kvp.Key;
            if (kvp.Value != 1) continue;

            int neighborCount = 0;
            Vector2Int[] dirs = {
            new Vector2Int(maxDistance, 0),
            new Vector2Int(-maxDistance, 0),
            new Vector2Int(0, maxDistance),
            new Vector2Int(0, -maxDistance)
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
                new Vector2Int(maxDistance, 0),
                new Vector2Int(-maxDistance, 0),
                new Vector2Int(0, maxDistance),
                new Vector2Int(0, -maxDistance)
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

    List<List<Vector2>> BuildTightClusters(Dictionary<Vector2Int, int> grid)
    {
        List<List<Vector2>> clusters = new List<List<Vector2>>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        foreach (var kvp in grid)
        {
            Vector2Int start = kvp.Key;
            if (kvp.Value != 1 || visited.Contains(start))
                continue;

            List<Vector2> cluster = new List<Vector2>();
            Queue<Vector2Int> toCheck = new Queue<Vector2Int>();
            toCheck.Enqueue(start);
            visited.Add(start);

            while (toCheck.Count > 0)
            {
                Vector2Int current = toCheck.Dequeue();
                cluster.Add(current);

                // Schritt 1: nach links gehen, um Startpunkt der Reihe zu finden
                Vector2Int left = current;
                while (grid.ContainsKey(new Vector2Int(left.x - maxDistance, left.y)) &&
                       grid[new Vector2Int(left.x - maxDistance, left.y)] == 1 &&
                       !visited.Contains(new Vector2Int(left.x - maxDistance, left.y)))
                {
                    left = new Vector2Int(left.x - maxDistance, left.y);
                }

                // Schritt 2: von dort nach rechts laufen
                Vector2Int xPoint = left;
                while (grid.ContainsKey(xPoint) && grid[xPoint] == 1)
                {
                    if (!visited.Contains(xPoint))
                    {
                        cluster.Add(xPoint);
                        visited.Add(xPoint);

                        // Schritt 3: nach oben (Y+) prüfen
                        Vector2Int up = new Vector2Int(xPoint.x, xPoint.y + maxDistance);
                        while (grid.ContainsKey(up) && grid[up] == 1 && !visited.Contains(up))
                        {
                            cluster.Add(up);
                            visited.Add(up);
                            toCheck.Enqueue(up);
                            up = new Vector2Int(up.x, up.y + maxDistance);
                        }

                        // Schritt 4: nach unten (Y-) prüfen
                        Vector2Int down = new Vector2Int(xPoint.x, xPoint.y - maxDistance);
                        while (grid.ContainsKey(down) && grid[down] == 1 && !visited.Contains(down))
                        {
                            cluster.Add(down);
                            visited.Add(down);
                            toCheck.Enqueue(down);
                            down = new Vector2Int(down.x, down.y - maxDistance);
                        }
                    }

                    // nach rechts gehen
                    xPoint = new Vector2Int(xPoint.x + maxDistance, xPoint.y);
                }
            }

            // Cluster speichern
            clusters.Add(cluster.Select(v => new Vector2(v.x, v.y)).ToList());
        }

        return clusters;
    }


    /*List<List<Vector2>> BuildTightClusters(Dictionary<Vector2Int, int> grid)
    {
        List<List<Vector2>> clusters = new List<List<Vector2>>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        foreach (var kvp in grid)
        {
            Vector2Int start = kvp.Key;
            if (kvp.Value != 1 || visited.Contains(start))
                continue;

            List<Vector2> cluster = new List<Vector2>();
            Queue<Vector2Int> toCheck = new Queue<Vector2Int>();
            toCheck.Enqueue(start);
            visited.Add(start);

            while (toCheck.Count > 0)
            {         
                Vector2Int nextx = kvp.Key;

                Vector2Int current = toCheck.Dequeue();   
                cluster.Add(new Vector2(current.x, current.y));

                // Gehe entlang X-Achse in beide Richtungen
                while (toCheck.Contains(nextx))
                {
                    Vector2Int nextStart = new Vector2Int(nextx.x - maxDistance, nextx.y);
                    while (toCheck.Contains(nextStart))
                    {
                        nextx = nextStart;
                        nextStart += new Vector2Int(-maxDistance, 0);
                    }

                    Vector2Int nexty = new Vector2Int(kvp.Key.x, kvp.Key.y + maxDistance);
                    while (toCheck.Contains(nexty))
                    {
                        nexty = new Vector2Int(nexty.x, nexty.y + maxDistance);
                        cluster.Add((Vector2)nexty);
                        visited.Add(nexty);
                        toCheck.Enqueue(nextx);
                    }
                    nexty = new Vector2Int(nextx.x, nextx.y -  maxDistance);
                    while (toCheck.Contains(nexty))
                    {
                        nexty = new Vector2Int(nexty.x, nexty.y - maxDistance);
                        cluster.Add((Vector2)nexty);
                        visited.Add(nexty);
                        toCheck.Enqueue(nextx);
                    }
                    cluster.Add((Vector2)nextx);
                    visited.Add(nextx);
                    toCheck.Enqueue(nextx);
                    nextx = new Vector2Int(nextx.x + maxDistance, nextx.y);
                }
                for (int dirX = -1; dirX <= 1; dirX += 2)
                {
                    nextx = new Vector2Int(current.x + dirX * maxDistance, current.y);
                    while (grid.ContainsKey(nextx) && grid[nextx] == 1 && !visited.Contains(nextx))
                    {
                        visited.Add(nextx);
                        toCheck.Enqueue(nextx);
                        nextx = new Vector2Int(nextx.x + dirX * maxDistance, nextx.y);
                    }
                }

                // Gehe entlang Z-Achse (bzw. Y in 2D-Grid) in beide Richtungen
                for (int dirY = -1; dirY <= 1; dirY += 2)
                {
                    nextx = new Vector2Int(current.x, current.y + dirY * maxDistance);
                    while (grid.ContainsKey(nextx) && grid[nextx] == 1 && !visited.Contains(nextx))
                    {
                        visited.Add(nextx);
                        toCheck.Enqueue(nextx);
                        nextx = new Vector2Int(nextx.x, nextx.y + dirY * maxDistance);
                    }
                }
            }

            clusters.Add(cluster);
        }

        return clusters;
    }*/


    void CreateCancelMesh(List<Vector2> hull)
    {
        if (hull.Count < 3) return;

        hull.Reverse();

        // 1?? Vertices erzeugen
        Vector3[] vertices = new Vector3[hull.Count];
        for (int i = 0; i < hull.Count; i++)
        {
            float h = IsTerrainInScene ? terrain.SampleHeight(new Vector3(hull[i].x, 0, hull[i].y)) : 0f;
            vertices[i] = new Vector3(hull[i].x, h + yOffset, hull[i].y);
        }

        // 2?? Triangulation (für konvexe Polygone einfaches Fan-Muster)
        List<int> triangles = new List<int>();
        for (int i = 1; i < hull.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        // 3?? Mesh erstellen
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // 4?? GameObject mit MeshRenderer erzeugen
        GameObject area = new GameObject("CancelAreaMesh", typeof(MeshFilter), typeof(MeshRenderer));
        area.GetComponent<MeshFilter>().mesh = mesh;

        // 5?? Material setzen
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
        foreach (KeyValuePair<Vector2Int, int> pair in spawnGrid)
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
    
        /// <summary>
        /// Findet Edge-Points (value==1) und gruppiert sie in zusammenhängende Cluster.
        /// - cellStep: Abstand zwischen Zellen (dein "cellSize" / "maxDistance").
        /// - allowDiagonal: true = 8-neighborhood, false = 4-neighborhood.
        /// </summary>
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

            // Optional: falls du eine geordnete Kontur brauchst, kannst du cluster weiter bearbeiten.
            // Hier geben wir komplette, zusammenhängende Edge-Point-Sets zurück (un-ordered lists).
            return clusters;
        }
}