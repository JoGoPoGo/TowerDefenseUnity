using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TerminalManager : MonoBehaviour
{
    public TextMeshProUGUI zeitEnergie;
    // Start is called before the first frame update
    void Start()
    {
        int energie = ProgressManager.Instance.GetCollectedStars();
        zeitEnergie.text = "GESAMMELTE ZEITENERGIE: " + energie;
    }
}
