using UnityEngine;

public class SlowDownMusicTrigger : MonoBehaviour
{
    [Header("Ziel-Tonhöhe (Pitch)")]
    public float targetPitch = 0.3f; // 0.3 ist sehr langsam und tief

    // OnEnable wird automatisch von Unity aufgerufen, 
    // in exakt dem Moment, in dem dieses GameObject "aktiv" geschaltet wird.
    void OnEnable()
    {
        // Prüfen, ob die Musik überhaupt da ist, und dann den Befehl geben
        if (KeepMusicPlaying.instance != null)
        {
            KeepMusicPlaying.instance.PitchDownTo(targetPitch);
        }
        else
            Debug.Log("KeepMusicPlaying not found");
    }
}