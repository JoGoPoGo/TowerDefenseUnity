using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Catapult : Tower
{
    public GameObject wurfArm;
    

    // Update is called once per frame
    protected override void Shoot()
    {
        base.Shoot();
        if (!spawnScript.spawned)
        {
            StartCoroutine(ShootAnimation());
        }
        
    }

    IEnumerator ShootAnimation()
    {
        float elapsed = 0f;
        float duration = 0.3f;
        float angle = 45f;

        while (elapsed < duration) //nach vorne Kippen
        {
            float step = 1.5f * (angle / duration) * Time.deltaTime;
            wurfArm.transform.Rotate(Vector3.right * step);
            elapsed += 1.5f * Time.deltaTime;
        }
        yield return new WaitForSeconds(0.2f);
        while (elapsed > 0)
        {
            float step = (angle / duration) * Time.deltaTime;
            wurfArm.transform.Rotate(Vector3.right * step * -1);
            elapsed -= Time.deltaTime;
            yield return null;
        }
    }
}
