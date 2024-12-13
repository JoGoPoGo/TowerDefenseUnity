using TMPro; // Falls du TextMeshPro verwendest
using TMPro.Examples;
using UnityEngine;

public class CoinDisplay : MonoBehaviour
{ 
    public TextMeshProUGUI coinText; // Referenz zum Text-Element
    public GameManager gameManager;

    // Methode, um die Anzeige zu aktualisieren
    void Update()
    {
        if(gameManager == null)
        {
            Debug.Log("GameManager");
            return;
        }
        coinText.text = "Credits: " + 0 + gameManager.credits;
    }
}

