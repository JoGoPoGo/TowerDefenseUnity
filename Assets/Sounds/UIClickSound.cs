using UnityEngine;
using UnityEngine.UI; // Wichtig: Damit Unity weiß, was ein UI-Button ist

[RequireComponent(typeof(Button))]
public class UIClickSound : MonoBehaviour
{
    [Header("Dein Klick-Sound:")]
    public AudioClip clickClip;

    // Wir nutzen EINEN einzigen Audio-Player für alle Buttons, um Chaos zu vermeiden
    private static AudioSource sfxPlayer;

    void Start()
    {
        // Wenn es noch keinen UI-Audio-Player gibt, erstellen wir schnell einen
        if (sfxPlayer == null)
        {
            GameObject sfxObj = new GameObject("UI_SoundPlayer");
            sfxPlayer = sfxObj.AddComponent<AudioSource>();

            // Verhindert, dass der Klick-Sound beim Szenenwechsel abgehackt wird!
            DontDestroyOnLoad(sfxObj);
        }

        // Wir sagen dem Button: "Wenn du geklickt wirst, spiele den Sound ab"
        // So musst du im Inspector keine Events mehr manuell verknüpfen!
        GetComponent<Button>().onClick.AddListener(PlaySound);
    }

    void PlaySound()
    {
        // PlayOneShot sorgt dafür, dass sich Sounds auch überlappen können (bei schnellem Klicken)
        sfxPlayer.PlayOneShot(clickClip);
    }
}