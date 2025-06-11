using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : Tower
{
    
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
        
    }
}
