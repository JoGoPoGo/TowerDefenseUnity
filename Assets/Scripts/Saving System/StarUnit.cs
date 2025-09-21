using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarUnit : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] Stars;
    public GameObject StarController;
    public int starsToShow;


    private bool yeah = false;

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf && yeah == false)
        {
            StartCoroutine(ShowStars());
            yeah = true;
        }
    }
    IEnumerator ShowStars()
    {
        foreach (GameObject Star in Stars)
        {
            Star.SetActive(false);
        }
        yield return null;
        ShowStars StarScript = StarController.GetComponent<ShowStars>();
        starsToShow = StarScript.totalStars;
        int counter = starsToShow;
        foreach (GameObject Star in Stars)
        { 
            if(counter > 0)
            {
                Star.SetActive(true);
            }
            counter--;
            yield return new WaitForSeconds(0.3f);
        }

    }
}
