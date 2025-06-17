using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpecial : DamageTest
{
    public GameObject splittingPrefab;
    public int splittCount;
    // Start is called before the first frame update

    // Update is called once per frame
    public override void Die(bool didDamage)
    {
        {
            if (!didDamage)
            {
                gameManager.AddCredits(reward);
            }
            if (IsOnlyEnemy() && isLast)
            {
                //SceneManager.LoadScene("LevelAuswahl");   //Auskommentieren, falls es zu unerwünschten Szenenwechsel kommt
                GameObject parent = GameObject.Find("Canvas");
                if (parent != null)
                {
                    Transform winTransform = parent.transform.Find("WinScreen");
                    if (winTransform != null)
                    {
                        winTransform.gameObject.SetActive(true);
                    }
                }
            }
            isAlive = false;
            Destroy(gameObject);
        }
    }
}
