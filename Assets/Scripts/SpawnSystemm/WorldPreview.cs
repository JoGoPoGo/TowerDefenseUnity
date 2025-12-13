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

    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = rangeMaterial;
    }

    // === EXTERNER AUFRUF ===
    // Wird vom Tower oder Controller aufgerufen
    public void ShowRange(Vector3 worldPosition, Quaternion rotation, float towerRange)
    {
        originPosition = worldPosition;
        baseRotation = rotation;
        range = towerRange;

        GenerateMesh();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void GenerateMesh()
    {
        Vector3 origin = originPosition + Vector3.up * 0.3f;

        float angleStep = previewAngle / (float)rayCount;
        float startAngle = -previewAngle * 0.5f;

        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = transform.InverseTransformPoint(origin);

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector3 localDir = AngleToDirection(angle);
            Vector3 worldDir = baseRotation * localDir;

            Vector3 point;
            if (Physics.Raycast(origin, worldDir, out RaycastHit hit, range, obstacleMask))
                point = hit.point;
            else
                point = origin + worldDir * range;

            vertices[i + 1] = transform.InverseTransformPoint(point);
        }

        for (int i = 0; i < rayCount; i++)
        {
            int idx = i * 3;
            triangles[idx] = 0;
            triangles[idx + 1] = i + 1;
            triangles[idx + 2] = i + 2;
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
