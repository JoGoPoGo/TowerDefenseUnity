using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]


public class DynamicRangeVisualizer : MonoBehaviour
{
    public int rayCount = 180; // Auflösung (mehr = glatterer Kreis)
    public Tower towerScript;
    public LayerMask obstacleMask; // Hindernisse
    public Material rangeMaterial;


    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private Mesh mesh;
    private float range;
    private SpawnOnMouseClick spawnOnMouseClick;

    private Vector3 lastPosition;


    void Awake()
    {
        // Mesh-Komponenten holen
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        // Eigenes Mesh anlegen, falls noch keins existiert
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    void Start()
    {
        lastPosition = transform.position;
        range = towerScript.range;
        spawnOnMouseClick = towerScript.spawnScript;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = rangeMaterial;
    }

    void LateUpdate()
    {
        if (gameObject.CompareTag("Tower") == false)
        {
            meshRenderer.enabled = true;
            if (IsMoved())
            {
                GenerateRangeMesh();
                lastPosition = transform.position;
            }

        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    void GenerateRangeMesh()
    {
        Vector3 origin = transform.position + Vector3.up * 0.3f; // leicht über Boden
        float angleStep = 360f / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = new Vector3(0,0.1f,0); // Mittelpunkt (lokal)

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = i * angleStep;
            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));

            // Raycast prüft Hindernisse
            if (Physics.Raycast(origin, dir, out RaycastHit hit, range, obstacleMask))
            {
                vertices[i + 1] = transform.InverseTransformPoint(hit.point);
            }
            else
            {
                vertices[i + 1] = dir * range;
            }
        }

        for (int i = 0; i < rayCount; i++)
        {
            int vIndex = i + 1;
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = vIndex + 1;
            triangles[i * 3 + 2] = vIndex;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
    private bool IsMoved()
    {
        return transform.position != lastPosition;
    }
}
