using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealBot : DamageTest
{
    public int healRange ;
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
        // Neues Partikelobjekt erzeugen
        GameObject part = Instantiate(particlePrefab, transform.position, Quaternion.identity);

        // Um 90° um X-Achse drehen
        part.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // Dauer des Partikelsystems auslesen
        ParticleSystem ps = part.GetComponent<ParticleSystem>();
        float lifetime = ps != null ? ps.main.duration : 2f;

        // Warten bis die Partikel verschwunden sind
        yield return new WaitForSeconds(lifetime);

        // Danach löschen
        Destroy(part);
    }


}
