using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class AbilityScript : MonoBehaviour
{
    public GameObject PlayerVariant; //hier den ThirdPersonController_LITE reinziehen
    public Button FreezeButton;
    public Button PoisonButton;

    public ParticleSystem poisonParticle;
    public ParticleSystem iceParticle;

    [Header("Freeze")]
    public int FreezeRange;
    public int speedPercentage;
    public int FreezeDuration;
    public int FreezeAbklingzeit;

    [Header("Poison")]
    public int PoisonRange;
    public int tickDamage;
    public int ticks;
    public float tickRate;
    public int PoisonAbklingzeit;

    private int activePoisonCount = 0;

    public void Freeze()
    {
        Debug.Log("Freeze BUTTON CLICKED");
        StartCoroutine(FreezeRoutine());
    }

    public IEnumerator FreezeRoutine()
    {

        yield return null;

        FreezeButton.interactable = false;

        iceParticle.Play();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log(enemies.Length);
        Dictionary<GameObject, float> originalSpeeds = new Dictionary<GameObject, float>();

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            Vector3 direction = enemy.transform.position - PlayerVariant.transform.position;
            float distance = Vector3.Distance(enemy.transform.position, PlayerVariant.transform.position);
            Debug.Log($"Enemy {enemy.name} distance: {distance}");

            if (distance <= FreezeRange)
            {
                Debug.Log("FreeeZEEEE");
                EnemyScript damageScript = enemy.GetComponent<EnemyScript>();
                if (damageScript == null) continue;

                originalSpeeds[enemy] = damageScript.speed;
                damageScript.speed *= speedPercentage / 100f;
            }
        }
        yield return new WaitForSeconds(1);

        iceParticle.Stop();

        yield return new WaitForSeconds(FreezeDuration - 1);

        foreach (var kvp in originalSpeeds)
        {
            if (kvp.Key == null) continue;

            EnemyScript damageScript = kvp.Key.GetComponent<EnemyScript>();
            if (damageScript != null)
            {
                damageScript.speed = kvp.Value;
            }
        }

        yield return new WaitForSeconds(FreezeAbklingzeit);
        FreezeButton.interactable = true;
        yield return null;
    }

    public void Poison()
    {
        StartCoroutine(PoisonRoutine());
    }

    public IEnumerator PoisonRoutine()
    {
        poisonParticle.transform.position = PlayerVariant.transform.position;
        poisonParticle.Play();
        PoisonButton.interactable = false;
        yield return null;

        foreach(GameObject enemy in ObjectsInRange(PoisonRange, "Enemy", PlayerVariant.transform.position))
        {
            if (enemy == null) continue;

            activePoisonCount++;
            StartCoroutine(Poisoned(enemy));
        }
        yield return new WaitForSeconds(1);
        poisonParticle.Stop();
        yield return new WaitUntil(() => activePoisonCount == 0);
        yield return new WaitForSeconds(PoisonAbklingzeit);
    }

    IEnumerator Poisoned(GameObject enemy)
    {
        if (enemy == null)
        {
            activePoisonCount--;
            yield break;
        }
        EnemyScript damageScript = enemy.GetComponent <EnemyScript>();

        if (damageScript == null)
        {
            activePoisonCount--;
            yield break;
        }
        for (int i = ticks; i > 0; i--)
        {
            damageScript.TakeDamage(tickDamage);
            //Debug.Log("Took Damage " + (ticks - i + 1) + " times");
            yield return new WaitForSeconds(1f / tickRate);
        }

        activePoisonCount--;
    }

    public GameObject[] ObjectsInRange(float range, string tag, Vector3 position)
    {
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag(tag);
        List<GameObject> objectsInRange = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue; // falls Objekt zerstört wurde

            // Nur X/Z Distanz berücksichtigen
            Vector2 objPosXZ = new Vector2(obj.transform.position.x, obj.transform.position.z);
            Vector2 centerXZ = new Vector2(position.x, position.z);
            float distance = Vector2.Distance(objPosXZ, centerXZ);

            if (distance <= range)
            {
                objectsInRange.Add(obj);
            }
        }

        return objectsInRange.ToArray();
    }

}
