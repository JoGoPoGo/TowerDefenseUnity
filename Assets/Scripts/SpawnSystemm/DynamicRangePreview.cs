using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]


public class DynamicRangePreview : MonoBehaviour
{
    public int rayCount = 180; // Auflösung (mehr = glatterer Kreis)
    public int accuracy = 1;

    public Tower towerScript;
    public LayerMask obstacleMask; // Hindernisse
    public Material rangeMaterial;

    public int previewAngle = 360;

    public bool showActivated = false;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private Mesh mesh;
    private float range;

    private SpawnOnMouseClick spawnOnMouseClick;
    private CancelDictionaryProtoType dictionary;

    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private TypeCanon canon;


    void Awake()
    {
        // Mesh-Komponenten holen
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        // Eigenes Mesh anlegen, falls noch keins existiert
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        canon = GetComponent<TypeCanon>();

    }

    void Start()
    {
        lastPosition = transform.position;
        UpdateStats();
        spawnOnMouseClick = towerScript.spawnScript;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = rangeMaterial;
    }

    void LateUpdate()
    {
        bool isTower = gameObject.CompareTag("Tower");

        // --- Mesh anzeigen? ---
        bool shouldShowMesh = (!isTower) || (isTower && showActivated);

        if (shouldShowMesh)
        {
            meshRenderer.enabled = true;

            if (IsMoved() || WasRotated() || showActivated)
            {
                GenerateRangeMesh();
                lastRotation = transform.rotation;
                lastPosition = transform.position;
            }
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    public void UpdateStats()
    {
        range = towerScript.range;
    }


    void GenerateRangeMesh()
    {
        
        UpdateStats();

        Vector3 origin = transform.position + Vector3.up * 0.3f;

        float angleStep = previewAngle / (float)rayCount;
        float startAngle = -previewAngle * 0.5f;

        // Mesh-Daten vorbereiten
        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        // Mittelpunkt (lokale Koordinaten)
        vertices[0] = new Vector3(0f, 0.1f, 0f);

        // --- RAYCAST LOOP --------------------------------------------------------
        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startAngle + i * angleStep;

            // Richtung im lokalen Raum
            Vector3 localDir = AngleToDirection(angle);

            // Richtung in Weltkoordinaten
            Quaternion testRotation = Quaternion.Euler(0, -45, 0);
            Vector3 worldDir = transform.rotation * localDir;

            // Raycast
            vertices[i + 1] = GetVertexPosition(origin, worldDir, localDir);
        }

        // --- TRIANGLE SETUP ------------------------------------------------------
        BuildTriangles(triangles);

        // --- APPLY TO MESH -------------------------------------------------------
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    // Wandelt einen Winkel (in Grad) in eine lokale Richtung um
    Vector3 AngleToDirection(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
    }

    // Gibt die richtige Vertex-Position zurück (Raycast oder max. Range)
    Vector3 GetVertexPosition(Vector3 origin, Vector3 worldDir, Vector3 localDir)
    {
        if (Physics.Raycast(origin, worldDir, out RaycastHit hit, range, obstacleMask))
        {
            return transform.InverseTransformPoint(hit.point);
        }
        else
        {
            return localDir * range;
        }
    }

    // Füllt das Triangle-Array für ein Fan-Mesh
    void BuildTriangles(int[] triangles)
    {
        for (int i = 0; i < rayCount; i++)
        {
            int baseIndex = i * 3;
            int v = i + 1;

            triangles[baseIndex]     = 0;
            triangles[baseIndex + 1] = v;
            triangles[baseIndex + 2] = v + 1;
        }
    }



    private bool IsMoved()
    {
        return transform.position != lastPosition;
    }
    private bool WasRotated()
    {
        return transform.rotation != lastRotation;
    }
}
