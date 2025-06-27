using UnityEngine;
using UnityEngine.UI;

public class DisableButton : MonoBehaviour
{
    private Button button;
    public GameManager gameManager;
    public Tower Tower;

    void Start()
    {
        // Zugriff auf den Button-Komponenten
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("Es wurde keine Button-Komponente gefunden. Bitte stelle sicher, dass dieses Skript auf einem Button liegt. at DisableButton, 17");
        }
        
    }
    private void Update()
    {
        if (gameManager.credits < Tower.price)
            DisableThisButton();
        if (gameManager.credits >= Tower.price)
            EnableThisButton();
    }


    public void EnableThisButton()
    {
        if (button != null)
        {
            button.interactable = true; // Deaktiviert den Button
            //Debug.Log("Button wurde aktiviert.");
        }
    }
    public void DisableThisButton()
    {
        if (button != null)
        {
            button.interactable = false; // Deaktiviert den Button
            //Debug.Log("Button wurde deaktiviert.");
        }
    }
}
