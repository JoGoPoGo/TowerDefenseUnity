using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOnDistance: MonoBehaviour
{
    public string loadScene;      // Name der Szene
    public float distance = 5f;   // Distanz zum Auslösen
    public Transform player;      // Spieler Transform
    public ParticleSystem portal;
    public int wait;

    private bool sceneLoaded = false;

    void Update()
    {
        if (sceneLoaded || player == null)
            return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= distance)
        {
            StartCoroutine(Teleport());
        }
        //Debug.Log(dist);
    }
    IEnumerator Teleport()
    {
        sceneLoaded = true;
        portal.Play();
        yield return new WaitForSeconds(wait);
        SceneManager.LoadScene(loadScene);
    }
}
