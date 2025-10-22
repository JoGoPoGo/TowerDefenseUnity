using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TypeCanon : Tower
{
    public int rangeDegrees;
    
    private int rotateCounter = 0;
    private Quaternion startDirection;

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
        if (spawnScript.spawned && Input.GetKey(KeyCode.R) && rotateCounter == 0)
        {
            Debug.Log("spawned");
            transform.Rotate(0f,1, 0f);
            startDirection = transform.rotation;    //gleicht startDirection der, vom Spieler bestimmten, Richtung an 
        }
        if (!spawnScript.spawned)
        {
            if(rotateCounter == 0) 
                rotateCounter = 1;
            UpdateTarget();     //sucht das Ziel
            if (target == null)   //f�hrt nichts aus, wenn kein Ziel gefunden wurde
                return;

            // Turm dreht sich zum Ziel
            Vector3 direction = target.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
            transform.rotation = Quaternion.Euler(0f, smoothedRotation.eulerAngles.y, 0f);

            // Wenn die Zeit zum Schie�en gekommen ist, wird geschossen
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate; // Setze den Timer f�r den n�chsten Schuss
            }

            fireCountdown -= Time.deltaTime;
        }
    }

    // Update is called once per frame
    protected override void UpdateTarget()   
    {
        // Sucht nach allen Gegnern mit dem Tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        // Finde den n�chsten Gegner innerhalb der Reichweite
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                Vector3 dirToEnemy = (enemy.transform.position - transform.position).normalized;
                RaycastHit hit;  // Pr�ft, ob zwischen dem Turm und dem Ziel ein Hinderniss ist

                if (!Physics.Raycast(transform.position, dirToEnemy, out hit, range, obstacleMask))
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }
        }

        // Wenn ein Gegner gefunden wurde, setze ihn als Ziel
        if (nearestEnemy != null && shortestDistance <= range)
        {
            Vector3 toTarget = (nearestEnemy.transform.position - transform.position).normalized;
            Vector3 forward = startDirection * Vector3.forward;

            float angle = Vector3.Angle(forward, toTarget);
            if(angle <= rangeDegrees / 2f)
            {
                target = nearestEnemy;
            }
            else
            {
                target = null;
            }
        }
        else
        {
            target = null; // Kein Gegner in Reichweite
        }
    }
}
