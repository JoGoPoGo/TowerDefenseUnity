using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class MasterVolumeSlider : MonoBehaviour
{
    private Slider volumeSlider;

    void Start()
    {
        volumeSlider = GetComponent<Slider>();

        // 1. Lade die gespeicherte Lautstärke. 
        // Die "1f" am Ende ist der Standardwert (100%), falls das Spiel zum allerersten Mal gestartet wird.
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        // 2. Wende die geladene Lautstärke sofort an
        AudioListener.volume = savedVolume;

        // 3. Setze den Regler an die richtige Position
        volumeSlider.value = savedVolume;

        // Verbinde den Slider
        volumeSlider.onValueChanged.AddListener(ChangeVolume);
    }

    void ChangeVolume(float newValue)
    {
        // Ändere die Lautstärke
        AudioListener.volume = newValue;

        // SPEICHERN: Merke dir den neuen Wert dauerhaft unter dem Namen "MasterVolume"
        PlayerPrefs.SetFloat("MasterVolume", newValue);

        // Zur Sicherheit sofort auf die Festplatte schreiben
        PlayerPrefs.Save();
    }
}