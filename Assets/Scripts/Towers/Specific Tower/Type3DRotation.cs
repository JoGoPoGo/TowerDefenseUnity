using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type3DRotation : Tower
{
    public GameObject CanonBase;
    public GameObject RotatePoint;

    protected override void RotateTo(GameObject t)
    {
        Vector3 targetPos = t.transform.position + new Vector3(0f, 2f, 0f);
        Vector3 direction = targetPos - RotatePoint.transform.position;
        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);

        RotatePoint.transform.rotation = Quaternion.Lerp(
            RotatePoint.transform.rotation,
            lookRotation,
            Time.deltaTime * turnSpeed
        );
    }

    protected override IEnumerator ProjectileAnimation(GameObject projectile, GameObject enemy)
    {
        float duration = 0.4f;
        float t = 0f;
        Vector3 startPos = projectile.transform.position;
        Vector3 targetPos = enemy.transform.position + new Vector3(0,2f,0);

        while (t < duration)
        {
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t / duration);
            projectile.transform.position = currentPos;
            t += Time.deltaTime;
            yield return null;
        }

        if (enemy != null)
            damageScript = enemy.GetComponent<EnemyScript>();
        if (damageScript != null)
            hitEnemy(damageScript);

        Destroy(projectile);
    }
}
