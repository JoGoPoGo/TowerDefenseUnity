using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using DG.Tweening; // WICHTIG: DOTween importieren!

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("UI Referenzen")]
    public GameObject pauseMenuBackground;       // Das Haupt-Panel (nur der dunkle Hintergrund)
    public UIPanelController buttonsContainer;   // NEU: Nutzt jetzt dein Animations-Skript!
    public UIPanelController optionsMenuUI;      // NEU: Nutzt jetzt dein Animations-Skript!

    [Header("Einstellungen")]
    public string raumschiffSzene = "NameDeinerRaumschiffSzene";

    [Header("Reset Event")]
    public UnityEvent onResetEvent;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                // Abfrage, ob wir gerade in den Optionen sind (wir prüfen das GameObject des Skripts)
                if (optionsMenuUI.gameObject.activeSelf)
                {
                    CloseOptions();
                }
                else
                {
                    Resume();
                }
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        //Time.timeScale = 1f;
        GameIsPaused = false;

        // Schließt die sichtbaren Panels mit deiner Animation
        buttonsContainer.Close();
        if (optionsMenuUI.gameObject.activeSelf)
        {
            optionsMenuUI.Close();
        }

        // Trick: Der dunkle Hintergrund wird erst nach Ablauf deiner Animations-Dauer deaktiviert!
        // Das true am Ende bedeutet: SetUpdate(true) -> ignoriert die pausierte Zeit
        DOVirtual.DelayedCall(buttonsContainer.duration, () => {
            pauseMenuBackground.SetActive(false);
        }, true);
    }

    void Pause()
    {
        GameIsPaused = true;
        //Time.timeScale = 0f;

        pauseMenuBackground.SetActive(true); // Dunkler Hintergrund geht sofort an

        buttonsContainer.Open(); // Buttons ploppen mit deiner Animation auf
    }

    // --- BUTTON FUNKTIONEN ---

    public void LoadRaumschiff()
    {
        //Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene(raumschiffSzene);
    }

    public void OpenOptions()
    {
        buttonsContainer.Close(); // Buttons animieren weg
        optionsMenuUI.Open();     // Optionen ploppen auf
    }

    public void CloseOptions()
    {
        optionsMenuUI.Close();    // Optionen animieren weg
        buttonsContainer.Open();  // Buttons ploppen wieder auf
    }

    public void ResetAction()
    {
        Debug.Log("Reset");
        onResetEvent.Invoke();
        Resume();
        SceneManager.LoadScene(raumschiffSzene);
    }

    public void QuitGame()
    {
        Debug.Log("Spiel wird verlassen...");
        Application.Quit();
    }
}