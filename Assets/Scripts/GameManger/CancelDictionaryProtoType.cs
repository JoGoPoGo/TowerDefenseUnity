using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class CancelDictionaryProtoType : MonoBehaviour
{
    private Dictionary<Vector2Int, int> spawnGrid = new Dictionary<Vector2Int, int>();

    public float yOffset = 0.05f;

    private Terrain terrain;
    private bool IsTerrainInScene;

    private int tiling;
    private SpawnOnMouseClick spawnScript;
    private int maxDistance;

    public Material cancelAreaMaterial;



    void Start()
    {
        spawnScript = FindObjectOfType<SpawnOnMouseClick>();
        tiling = spawnScript.tiling;
        maxDistance = tiling;

        terrain = FindObjectOfType<Terrain>();
        IsTerrainInScene = (terrain != null);
    }

    public void hideCancelArea()
    {
        // Hole den MeshFilter der Komponente
        MeshFilter mf = GetComponent<MeshFilter>();

        // Setze das Mesh auf null, um die Anzeige auszublenden
        if (mf != null)
        {
            mf.mesh = null;
        }

        // Optional: Die ursprüngliche Aufräum-Logik kann bleiben, falls Sie 
        // das Haupt-GameObject später für andere Zwecke verwenden (z.B. Outlines)
        // oder falls das alte System noch irgendwo ein Kind-Objekt erzeugt hat.
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void showCancelArea()
    {
        ShowAreaFilled(spawnGrid);
    }

    public void ShowAreaFilled(Dictionary<Vector2Int, int> grid)
    {
        // Aufräumen: Löscht nur die Kinder, falls sie noch da sind (obwohl die Mesh-Lösung dies nicht mehr erfordert)
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // 1. Sammle alle besetzten Positionen (bleibt gleich)
        List<Vector2Int> occupiedPositions = new List<Vector2Int>();
        foreach (var kvp in grid)
        {
            if (kvp.Value == 1)
            {
                occupiedPositions.Add(kvp.Key);
            }
        }

        // 2. Erstelle das kombinierte Mesh (NEU)
        Mesh combinedMesh = CreateCombinedQuadMesh(occupiedPositions, tiling, yOffset, IsTerrainInScene);

        MeshFilter mf = GetComponent<MeshFilter>();
        MeshRenderer mr = GetComponent<MeshRenderer>();

        mf.mesh = combinedMesh;

        // 3. Material zuweisen (jetzt direkt am Haupt-GameObject)
        if (cancelAreaMaterial != null)
        {
            mr.material = cancelAreaMaterial;
        }
        else
        {
            // Fallback-Material-Logik... (Beibehalten)
            mr.material = new Material(Shader.Find("Unlit/Transparent"));
            mr.material.color = new Color(1f, 0f, 0f, 0.5f);
        }
    }

    // ---- Neue Hilfsfunktion für die Mesh-Erstellung ----
    // ---- Neue Hilfsfunktion für die Mesh-Erstellung ----
    // ---- Neue Hilfsfunktion für die Mesh-Erstellung ----
    private static Mesh CreateCombinedQuadMesh(List<Vector2Int> positions, int tiling, float yOffset, bool isTerrainInScene)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        Terrain activeTerrain = isTerrainInScene ? Terrain.activeTerrain : null;

        float halfTiling = tiling / 2f; // Konstante für die halbe Kachelgröße

        // Für jeden Gitterpunkt ein Quad erstellen
        foreach (Vector2Int pos in positions)
        {
            // Index des ersten Vertex für dieses Quad
            int vertexIndex = vertices.Count;

            // 1. Terrain-Höhe bestimmen
            float baseHeight = 0;
            // Für SampleHeight muss der korrigierte Mittelpunkt an das Terrain übergeben werden!
            Vector3 centerPos = new Vector3(pos.x + halfTiling, 0, pos.y + halfTiling);

            if (isTerrainInScene && activeTerrain != null)
            {
                // Terrain.SampleHeight verwendet World-Koordinaten, wir sampeln in der Mitte der Kachel.
                baseHeight = activeTerrain.SampleHeight(centerPos);
            }
            float height = baseHeight + yOffset;

            // NEUE BASIS-POSITIONEN FÜR DIE VERTEX-BERECHNUNG
            // Dies ist der Mittelpunkt der Gitterzelle
            float centerX = pos.x + halfTiling;
            float centerZ = pos.y + halfTiling;


            // 2. Vertices (Eckpunkte) berechnen: 
            // Berechne die Eckpunkte relativ zum ZENTRUM (centerX, centerZ)
            // Dies ist die robusteste Methode, um sicherzustellen, dass das Quad
            // zentriert ist, unabhängig davon, ob pos die untere linke Ecke ist.
            vertices.Add(new Vector3(centerX - halfTiling, height, centerZ - halfTiling)); // Bottom-Left (0)
            vertices.Add(new Vector3(centerX + halfTiling, height, centerZ - halfTiling)); // Bottom-Right (1)
            vertices.Add(new Vector3(centerX + halfTiling, height, centerZ + halfTiling)); // Top-Right (2)
            vertices.Add(new Vector3(centerX - halfTiling, height, centerZ + halfTiling)); // Top-Left (3)


            // 3. Triangles (Dreiecke) für das Quad (unverändert)
            triangles.Add(vertexIndex + 0);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);

            triangles.Add(vertexIndex + 0);
            triangles.Add(vertexIndex + 3);
            triangles.Add(vertexIndex + 2);

            // 4. UVs (unverändert)
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals(); // Wichtig für die Beleuchtung
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
}