using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowStars : MonoBehaviour
{
    public GameObject Base;
    public TMP_Text display;

    public static int collectedStars;

    private BaseHealth baseSkript;
    // Start is called before the first frame update
    void Start()
    {
        baseSkript = Base.GetComponent<BaseHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            display.text = "Stars:" + baseSkript.receivedStars;
            collectedStars = collectedStars + baseSkript.receivedStars;
        }
    }
}
