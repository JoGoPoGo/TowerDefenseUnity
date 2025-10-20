using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int credits = 0;
    
    public bool SpendCredits(int amount, bool spend = true)
    {
        if(credits >= amount)
        {
            if (spend)
            {
                credits -= amount; //zieht amount von credits ab
            }
            return true;
        }
        return false;
    }

    public void AddCredits(int amount)
    {
        credits += amount;
    }

    public void RemoveCredits(int amount)
    {
        credits -= amount;
    }
}
