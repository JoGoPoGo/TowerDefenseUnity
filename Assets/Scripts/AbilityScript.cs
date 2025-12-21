using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityScript : MonoBehaviour
{
    public GameObject PlayerVariant; //hier den ThirdPersonController reinziehen
    public Button FreezeButton;

    [Header("Freeze")]
    public int FreezeRange;
    public int speedPercentage;
    public int FreezeDuration;
    public int Abklingzeit;

    public void Freeze()
    {
        Debug.Log("Freeze BUTTON CLICKED");
        StartCoroutine(FreezeRoutine());
    }

    public IEnumerator FreezeRoutine()
    {

        yield return null;

        FreezeButton.interactable = false;
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
                DamageTest damageScript = enemy.GetComponent<DamageTest>();
                if (damageScript == null) continue;

                originalSpeeds[enemy] = damageScript.speed;
                damageScript.speed *= speedPercentage / 100f;
            }
        }

        yield return new WaitForSeconds(FreezeDuration);

        foreach (var kvp in originalSpeeds)
        {
            if (kvp.Key == null) continue;

            DamageTest damageScript = kvp.Key.GetComponent<DamageTest>();
            if (damageScript != null)
            {
                damageScript.speed = kvp.Value;
            }
        }

        yield return new WaitForSeconds(Abklingzeit);
        FreezeButton.interactable = true;
        yield return null;
    }

}
