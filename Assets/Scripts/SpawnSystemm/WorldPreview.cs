using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WorldPreview : MonoBehaviour
{
    public int rayCount = 180;
    public int previewAngle = 360;
    public LayerMask obstacleMask;
    public float heightOffset = 0.05f;

    private float range;
    private float minRange;

    private Vector3 originPosition;
    private Quaternion baseRotation;

    private LineRenderer lineRenderer;
    private Terrain terrain;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        terrain = Terrain.activeTerrain;

        lineRenderer.loop = true;
        lineRenderer.positionCount = rayCount + 1;
    }

    // === EXTERNER AUFRUF ===
    public void ShowRange(
        Vector3 worldPosition,
        Quaternion rotation,
        float towerRange,
        float towerMinRange,
        int towerPreviewAngle)
    {
        lineRenderer.enabled = true;

        originPosition = worldPosition;
        baseRotation = rotation;
        range = towerRange;
        minRange = Mathf.Max(0f, towerMinRange);
        previewAngle = towerPreviewAngle;

        GenerateOutline();
    }

    public void Hide()
    {
        lineRenderer.enabled = false;
    }

    void GenerateOutline()
    {
        if (terrain == null) return;

        Vector3 origin = originPosition + Vector3.up * 2f;

        float angleStep = previewAngle / (float)rayCount;
        float startAngle = -previewAngle * 0.5f;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startAngle + i * angleStep;

            Vector3 localDir = AngleToDirection(angle);
            Vector3 worldDir = baseRotation * localDir;

            Vector3 endPoint;

            // Raycast für Hindernisse
            if (Physics.Raycast(origin, worldDir, out RaycastHit hit, range, obstacleMask))
                endPoint = hit.point;
            else
                endPoint = origin + worldDir * range;

            // --- Terrain-Anpassung ---
            float terrainHeight = terrain.SampleHeight(endPoint)
                                + terrain.transform.position.y;

            endPoint.y = terrainHeight + heightOffset;

            lineRenderer.SetPosition(i, endPoint);
        }
    }

    Vector3 AngleToDirection(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
    }
}