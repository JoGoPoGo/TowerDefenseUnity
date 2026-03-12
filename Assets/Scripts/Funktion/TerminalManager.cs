using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TerminalManager : MonoBehaviour
{
    public TextMeshProUGUI zeitEnergie;
    public GameObject clock;
    public GameObject missionButton;
    private int energie;
    // Start is called before the first frame update
    void Start()
    {
        energie = ProgressManager.Instance.GetCollectedStars();
        zeitEnergie.text = "GESAMMELTE ZEITENERGIE: " + energie;
    }
    private void Update()
    {
        if (zeitEnergie.gameObject.activeSelf)
        {
            clock.SetActive(true);
            missionButton.SetActive(true);
        }
        else
        {
            clock.SetActive(false);
            missionButton.SetActive(false);
        }
        if(ProgressManager.Instance.GetCollectedStars() > energie)
        {
            zeitEnergie.text = "GESAMMELTE ZEITENERGIE: " + ProgressManager.Instance.GetCollectedStars();
        }
    }
}
