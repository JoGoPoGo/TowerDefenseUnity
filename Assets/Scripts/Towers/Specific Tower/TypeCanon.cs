using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TypeCanon : Tower
{
    [Header("Changebles")]
    public int rangeDegrees;

    [Header("Funktion")]
    private int rotateCounter = 0;
    public Quaternion startDirection;
    private float rotateWaiter = 0;

    private DynamicRangePreview rangePreviewScript;
    // Start is called before the first frame update



    protected override void Start()
    {
        base.Start();
        rangePreviewScript = GetComponent<DynamicRangePreview>();
        rangePreviewScript.previewAngle = rangeDegrees;
    }

    protected override void Update()
    {
        if (!spawnScript.spawned)
        {
            base.Update();
            return;
        }

        // Taste gedrückt ? Timer starten
        if (Input.GetKeyDown(KeyCode.R))
        {
            rotateWaiter = 0f;
        }

        // Taste gehalten ? langsame Rotation + Timer erhöhen
        if (Input.GetKey(KeyCode.R) && rotateCounter == 0)
        {
            transform.Rotate(0f, 1f, 0f);
            startDirection = transform.rotation;

            rotateWaiter += Time.deltaTime;
        }

        // Taste losgelassen ? prüfen ob kurzer Klick
        if (Input.GetKeyUp(KeyCode.R))
        {
            if (rotateWaiter < 0.2f)
            {
                transform.Rotate(0f, 90f, 0f);
                startDirection = transform.rotation;
            }

            rotateWaiter = 0f; // IMMER resetten
        }

        base.Update();
    }

    // Update is called once per frame
    protected override void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        Vector3 forward = startDirection * Vector3.forward;

        foreach (GameObject enemy in enemies)
        {
            Vector3 dir = enemy.transform.position - transform.position;
            float distance = dir.magnitude;

            // Direkt Reichweite prüfen
            if (distance > range)
                continue;

            // Richtung normalisieren
            Vector3 dirNormalized = dir.normalized;

            // Winkel prüfen
            float angle = Vector3.Angle(forward, dirNormalized);
            if (angle > rangeDegrees / 2f)
                continue;

            // Raycast nur bis zum Gegner!
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dirNormalized, out hit, distance, obstacleMask))
                continue;

            // Jetzt erst Distanz vergleichen
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        target = nearestEnemy;
    }
}
