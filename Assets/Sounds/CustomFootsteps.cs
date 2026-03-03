using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CustomFootsteps : MonoBehaviour
{
    [Header("Ziehe hier deine Schritt-Sounds rein")]
    public AudioClip[] footstepClips;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Diese Methode rufen wir gleich direkt aus der Animation heraus auf!
    public void DoPlayFootstepSound()
    {
        if (footstepClips != null && footstepClips.Length > 0)
        {
            // Einen zufälligen Sound aus der Liste picken
            int randomIndex = Random.Range(0, footstepClips.Length);
            AudioClip clip = footstepClips[randomIndex];

            // Minimaler Zufalls-Pitch und Lautstärke, damit es nicht wie eine Maschine klingt
            audioSource.pitch = Random.Range(0.85f, 1.15f);
            audioSource.volume = Random.Range(0.6f, 0.9f);

            // Sound abspielen
            audioSource.PlayOneShot(clip);
        }
    }
}