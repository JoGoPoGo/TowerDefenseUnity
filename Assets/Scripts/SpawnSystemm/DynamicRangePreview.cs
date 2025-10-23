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

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private Mesh mesh;
    private float range;

    private SpawnOnMouseClick spawnOnMouseClick;
    private CancelDictionary dictionary;

    private Vector3 lastPosition;
    private Quaternion lastRotation; 


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
            if (IsMoved() || WasRotated())
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

    void GenerateRangeMesh()
    {
        //if (rayCount <= 0) return;

        Vector3 origin = transform.position + Vector3.up * 0.3f; // leicht über Boden

        float angleStep = (float)previewAngle / (float)rayCount;

        float startAngle =  -previewAngle * 0.5f;

        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = new Vector3(0f, 0.1f, 0f); // Mittelpunkt (lokal)

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startAngle + i * angleStep; // 0..previewAngle
            Vector3 localDir = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0 , Mathf.Cos(angle * Mathf.Deg2Rad));


            Vector3 worldDir = transform.rotation * localDir;

            // Debug (sichtbar im Scene-View, nur beim Entwickeln)
            Debug.DrawRay(origin, worldDir * Mathf.Min(range, 10f), Color.yellow, 0.1f);

            if (Physics.Raycast(origin, worldDir, out RaycastHit hit, range, obstacleMask))
            {
                vertices[i + 1] = transform.InverseTransformPoint(hit.point);
                // Debug: roter Trefferpunkt
                Debug.DrawLine(origin, hit.point, Color.red, 0.1f);
            }
            else
            {
                vertices[i + 1] = localDir * range;
            }
        }

        for (int i = 0; i < rayCount; i++)
        {
            int vIndex = i + 1;
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = vIndex;
            triangles[i * 3 + 2] = vIndex + 1;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // optional: bounds einstellen, falls Mesh außerhalb liegt
        mesh.RecalculateBounds();
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
