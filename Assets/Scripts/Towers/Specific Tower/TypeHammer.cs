using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeHammer : Tower
{
    [Header("Animation")]

    public GameObject Hammer;
    public GameObject HammerRing;

    protected override void Shoot()         // protected für Schussanimationen und Funktionen
    {  // Erzeugt das Projektil an der Feuerposition und weist ihm die Richtung des Ziels zu#
        if (!spawnScript.spawned)
        {
            damageScript = target.GetComponent<DamageTest>();
            damageScript.TakeDamage(damageAmount);
            if (canon != null)
            {
                Debug.Log("bola");
            }
            StartCoroutine(HammerAnimation());
        }
    }
    IEnumerator HammerAnimation()
    {
        Vector3 hammerStart = Hammer.transform.position;
        Vector3 hammerDown = hammerStart - new Vector3(0, 2f, 0);

        float downSpeed = 10f;    // Hammer fällt schnell
        float upSpeed = 2f;       // Hammer geht langsam hoch

        float ringDownSpeed = 360f; // Ring schnell, Uhrzeigersinn
        float ringUpSpeed = 60f;    // Ring langsam, gegen Uhrzeigersinn

        // 1️⃣ Hammer fällt runter, Ring dreht schnell
        while (Hammer.transform.position.y > hammerDown.y)
        {
            // Hammer nach unten
            Hammer.transform.position = Vector3.MoveTowards(
                Hammer.transform.position,
                hammerDown,
                downSpeed * Time.deltaTime
            );

            // Ring Rotation (Uhrzeigersinn)
            HammerRing.transform.Rotate(Vector3.forward, ringDownSpeed * Time.deltaTime, Space.Self);

            yield return null;
        }

        // 2️⃣ Hammer geht langsam hoch, Ring dreht langsam zurück
        while (Hammer.transform.position.y < hammerStart.y)
        {
            // Hammer hoch
            Hammer.transform.position = Vector3.MoveTowards(
                Hammer.transform.position,
                hammerStart,
                upSpeed * Time.deltaTime
            );

            // Ring Rotation (gegen Uhrzeigersinn)
            HammerRing.transform.Rotate(Vector3.forward, -ringUpSpeed * Time.deltaTime, Space.Self);

            yield return null;
        }

        // Sicherstellen, dass Hammer exakt wieder am Start ist
        Hammer.transform.position = hammerStart;
    }
}
