using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : Tower
{
    public Animator animator;
    
    protected override void Shoot()
    {
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
            Debug.Log("Shoot Trigger activated");
        }
        else
        {
            Debug.LogError("Animator not assigned!");
        }
        base.Shoot();  
    }
}
