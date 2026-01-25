using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeMoreCanons : Tower
{
    public GameObject[] firePoints;

    private int firePointCount;
    private int i = 0;

    protected override void Start()
    {
        base.Start();
        firePointCount = firePoints.Length;
        firePoint = firePoints[0];
    }

    protected override void Shoot()
    {
        base.Shoot();
        i++;
        if (i == firePointCount)
            i = 0;
        firePoint = firePoints[i];
    }
}
