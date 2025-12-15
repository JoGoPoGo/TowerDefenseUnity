using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPreview : MonoBehaviour
{
    public int rayCount = 180;
    public int previewAngle = 360;
    public LayerMask obstacleMask;
    public Material rangeMaterial;

    private Mesh mesh;
    private float range;

    private Vector3 originPosition;
    private Quaternion baseRotation;
    private float minRange;

    private MeshRenderer meshRenderer;

    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = rangeMaterial;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // === EXTERNER AUFRUF ===
    // Wird vom Tower oder Controller aufgerufen
    public void ShowRange(
    Vector3 worldPosition,
    Quaternion rotation,
    float towerRange,
    float towerMinRange,
    int towerPreviewAngle)
    {
        meshRenderer.enabled = true;
        originPosition = worldPosition;
        baseRotation = rotation;
        range = towerRange;
        minRange = Mathf.Max(0f, towerMinRange); // Sicherheit
        previewAngle = towerPreviewAngle;

        GenerateMesh();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
    }

    void GenerateMesh()
    {
        Vector3 origin = originPosition + Vector3.up * 0.3f;

        float angleStep = previewAngle / (float)rayCount;
        float startAngle = -previewAngle * 0.5f;

        // Pro Ray: inner + outer
        Vector3[] vertices = new Vector3[(rayCount + 1) * 2];
        int[] triangles = new int[rayCount * 6];

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector3 localDir = AngleToDirection(angle);
            Vector3 worldDir = baseRotation * localDir;

            // --- INNER POINT ---
            Vector3 innerPoint = origin + worldDir * minRange;
            vertices[i * 2] = transform.InverseTransformPoint(innerPoint);

            // --- OUTER POINT ---
            Vector3 outerPoint;
            if (Physics.Raycast(origin, worldDir, out RaycastHit hit, range, obstacleMask))
                outerPoint = hit.point;
            else
                outerPoint = origin + worldDir * range;

            vertices[i * 2 + 1] = transform.InverseTransformPoint(outerPoint);
        }

        // --- TRIANGLES ---
        int triIndex = 0;
        for (int i = 0; i < rayCount; i++)
        {
            int v = i * 2;

            triangles[triIndex++] = v;
            triangles[triIndex++] = v + 1;
            triangles[triIndex++] = v + 2;

            triangles[triIndex++] = v + 1;
            triangles[triIndex++] = v + 3;
            triangles[triIndex++] = v + 2;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    Vector3 AngleToDirection(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
    }
}
