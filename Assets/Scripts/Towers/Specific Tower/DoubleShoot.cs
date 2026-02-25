using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleShoot : Tower
{

    [Header("Changebles")]
    public int shootNumber = 2;
    public float secondFirerate;

    [Header("Funktion")]
    private bool shouldUpdateTarget = true;
    private int firerateCounter;

    protected override void Start()
    {
        base.Start();
        firerateCounter = shootNumber;
    }

    protected override void Update()
    {
        if(target == null)  //Wenn das Target verloren geht, z.B. stirbt, wird resetet.
        {
            shouldUpdateTarget = true;
            fireCountdown = 1f / fireRate;
            firerateCounter = shootNumber;
        }
        if (gameObject.CompareTag("Tower") && dictionaryActivater)
        {
            Vector2Int posi = new Vector2Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));

            OccupyPositionsInCircle(posi, spawnCancelRadius);
            dictionaryActivater = false;
        }

        if (UpdateCounter == 30)
        {
            UpdateCounter = 0;
        }

        if (gameObject.CompareTag("Tower") && shouldUpdateTarget)
        {
            UpdateTarget();
        }

        UpdateCounter++;

        if (target == null)   //führt nichts aus, wenn kein Ziel gefunden wurde
            return;

        // Turm dreht sich zum Ziel
        Vector3 direction = target.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        transform.rotation = Quaternion.Euler(0f, smoothedRotation.eulerAngles.y, 0f);

        // Wenn die Zeit zum Schießen gekommen ist, wird geschossen

        if (fireCountdown <= 0f)
        {

            if (gameObject.CompareTag("Tower"))      //fängt den Schuss ab, wenn der Turm noch nicht platziert wurde
            {
                if (!spawnScript.spawned)
                    audioSource.PlayOneShot(shootSound);
                Shoot();
            }

            if (firerateCounter <= 0)               //wenn oft genug nacheinander auf ein Ziel geschossen wurde, wird "Nachgeladen"
            {
                fireCountdown = 1f / fireRate; // Setze den Timer für den nächsten Schuss
                firerateCounter = shootNumber;
                shouldUpdateTarget = true;
            }
            else                                 //wenn nach nicht oft genug auf ein Ziel geschossen wurde, bleibt das Ziel und setzt den fireCountdown auf die zweite Firerate 
            {
                fireCountdown = 1f / secondFirerate;
                firerateCounter--;
                shouldUpdateTarget = false;
            }
        }

        fireCountdown -= Time.deltaTime;
    }
}
