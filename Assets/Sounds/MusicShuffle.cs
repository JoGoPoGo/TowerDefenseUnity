using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicShuffler : MonoBehaviour
{
    [Header("Ziehe hier deine Lieder rein:")]
    public AudioClip[] musicClips;

    private AudioSource audioSource;
    private int lastPlayedIndex = -1; // Merkt sich das letzte Lied, damit es nicht zweimal hintereinander läuft

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayRandomSong();
    }

    void Update()
    {
        // Prüft jeden Frame: Wenn die Musik aufgehört hat zu spielen, starte ein neues Lied
        if (!audioSource.isPlaying && musicClips.Length > 0)
        {
            PlayRandomSong();
        }
    }

    void PlayRandomSong()
    {
        // Wenn es nur ein Lied gibt, spiele es einfach ab
        if (musicClips.Length == 1)
        {
            audioSource.clip = musicClips[0];
            audioSource.Play();
            return;
        }

        // Wähle ein zufälliges Lied aus, das nicht das gleiche ist wie das vorherige
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, musicClips.Length);
        }
        while (randomIndex == lastPlayedIndex);

        // Speichere die Nummer des Liedes und spiele es ab
        lastPlayedIndex = randomIndex;
        audioSource.clip = musicClips[randomIndex];
        audioSource.Play();
    }
}