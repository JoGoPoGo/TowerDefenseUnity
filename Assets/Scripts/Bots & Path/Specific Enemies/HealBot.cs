using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealBot : DamageTest
{
    public int healRange;
    public int healAmount = 10;
    public float healIntervall = 2;

    public GameObject particlePrefab;

    private float startIntervall;

    private List<GameObject> members;

    protected override void Start()
    {
        base.Start();
        startIntervall = healIntervall;
    }

    protected override void Update()
    {
        base.Update();
        if(healIntervall <= 0)
        {
            Heal();
            healIntervall = startIntervall;
        }
        else
        {
            healIntervall -= Time.deltaTime;
        }
    }
    void Heal()
    {
        
        members = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        foreach (GameObject member in members)
        {
            Vector3 direction = member.transform.position - transform.position;
            float distance = direction.magnitude;
            if (distance <= healRange)
            {
                DamageTest botScript = member.GetComponent<DamageTest>();
                if (botScript.currentHealth + healAmount > botScript.maxHealth)
                {
                    botScript.TakeDamage(currentHealth - maxHealth);
                }
                else
                {
                    botScript.TakeDamage(-healAmount);
                }
                StartCoroutine(ParticlePlay());
                Debug.Log(member.name + " healed! New HP = " + botScript.currentHealth);
            }
        }
    }
    IEnumerator ParticlePlay()
    {
        ParticleSystem ps = particlePrefab.GetComponent<ParticleSystem>();

        if (ps != null) 
        {
            var main = ps.main;
            main.startLifetime = (float)healRange / 10f;

        }
        ps.Play();
        yield return new WaitForSeconds(3);
        ps.Stop();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healRange);
    }
}
