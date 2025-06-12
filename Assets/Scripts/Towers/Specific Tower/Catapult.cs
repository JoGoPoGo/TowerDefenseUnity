using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : Tower
{
    public GameObject wurfArm;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Shoot()
    {
        base.Shoot();
        ShootAnimation();
    }

    void ShootAnimation()
    {
        wurfArm.transform.Rotate(Vector2.right * Time.deltaTime * 45f);

    }
}
