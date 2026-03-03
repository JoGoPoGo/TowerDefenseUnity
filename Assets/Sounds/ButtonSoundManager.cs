using UnityEngine;
using UnityEngine.UI; // Wichtig für die Buttons

public class ButtonSoundManager : MonoBehaviour
{
    [Header("Dein Klick-Sound:")]
    public AudioClip clickSound;

    [Header("Ziehe hier alle deine Buttons rein:")]
    public Button[] uiButtons; // Die Liste deiner Buttons

    private AudioSource audioSource;

    void Start()
    {
        // 1. AudioSource für das UI erstellen oder holen
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Das Objekt unzerstörbar machen, damit der Sound beim Szenenwechsel nicht abbricht
        // (Achtung: Funktioniert nur, wenn dieses Skript auf einem Root-Objekt ganz oben liegt!)
        DontDestroyOnLoad(gameObject);

        // 2. Wir gehen jeden Button in deiner Liste durch...
        foreach (Button btn in uiButtons)
        {
            // Zur Sicherheit prüfen, ob das Feld im Inspector nicht versehentlich leer ist
            if (btn != null)
            {
                // ...und sagen dem Button: "Wenn du geklickt wirst, spiele diesen Sound!"
                btn.onClick.AddListener(PlayClickSound);
            }
        }
    }

    void PlayClickSound()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}