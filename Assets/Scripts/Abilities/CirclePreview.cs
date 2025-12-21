using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class CirclePreview : MonoBehaviour
{
    public int segments = 64;
    public float yOffset = 0.05f;

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.loop = true;
        lr.useWorldSpace = false;
    }

    public void Draw(float radius, Color color)
    {
        lr.startColor = color;
        lr.endColor = color;

        lr.positionCount = segments;

        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * angleStep * i;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            lr.SetPosition(i, new Vector3(x, yOffset, z));
        }
    }

    public void Hide()
    {
        lr.positionCount = 0;
    }

}
