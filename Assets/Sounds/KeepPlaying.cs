using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class KeepMusicPlaying : MonoBehaviour
{
    // Jetzt "public", damit andere Skripte dem Player Befehle geben können
    public static KeepMusicPlaying instance;

    [Header("Einstellungen")]
    public string stopSceneName = "Main Menu";
    public float fadeOutTime = 1.5f;
    public float pitchFadeTime = 1.0f; // Wie lange das Verlangsamen dauert

    private AudioSource audioSource;
    private bool isFading = false;
    private float originalPitch = 1f;  // Merkt sich die normale Geschwindigkeit
    private Coroutine pitchCoroutine;  // Speichert den laufenden Pitch-Vorgang

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        originalPitch = audioSource.pitch; // Normale Tonhöhe speichern (meistens 1.0)
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Pitch sofort zurücksetzen, wenn eine neue Szene geladen wird!
        if (pitchCoroutine != null)
        {
            StopCoroutine(pitchCoroutine);
        }
        audioSource.pitch = originalPitch;

        // 2. Prüfen, ob wir im Hauptmenü sind
        if (scene.name == stopSceneName && !isFading)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    // --- NEU: Wird von außen aufgerufen, um die Musik zu verlangsamen ---
    public void PitchDownTo(float targetPitch)
    {
        if (pitchCoroutine != null) StopCoroutine(pitchCoroutine);
        pitchCoroutine = StartCoroutine(FadePitchCoroutine(targetPitch));
    }

    // Der fließende Übergang für den Pitch
    // Der fließende Übergang für den Pitch
    private IEnumerator FadePitchCoroutine(float targetPitch)
    {
        float startPitch = audioSource.pitch;
        float elapsedTime = 0f;

        // Pitch schrittweise anpassen
        while (elapsedTime < pitchFadeTime)
        {
            // WICHTIG: unscaledDeltaTime ignoriert Time.timeScale = 0f !
            elapsedTime += Time.unscaledDeltaTime;
            audioSource.pitch = Mathf.Lerp(startPitch, targetPitch, elapsedTime / pitchFadeTime);
            yield return null;
        }

        audioSource.pitch = targetPitch; // Zur Sicherheit exakt auf den Zielwert setzen
    }

    // Den Volume-Fade passen wir zur Sicherheit auch direkt an, 
    // falls du mal aus einem pausierten Menü heraus ins Hauptmenü gehst.
    private IEnumerator FadeOutAndDestroy()
    {
        isFading = true;
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.unscaledDeltaTime / fadeOutTime;
            yield return null;
        }

        audioSource.volume = 0;
        instance = null;
        Destroy(gameObject);
    }
}