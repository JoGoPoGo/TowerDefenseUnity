using UnityEngine;
using DG.Tweening; // Wichtig: Der Namespace für DOTween

public class UIPanelController : MonoBehaviour
{
    [Header("Einstellungen")]
    public float duration = 0.4f;
    public Ease openEase = Ease.OutBack; // Schöner "Plopp"-Effekt
    public Ease closeEase = Ease.InBack; // Zieht sich kurz zurück und verschwindet

    public void Open()
    {
        // 1. Panel aktivieren
        gameObject.SetActive(true);

        // 2. Start-Skalierung auf 0 setzen
        transform.localScale = Vector3.zero;

        // 3. Auf 1 skalieren
        transform.DOScale(Vector3.one, duration).SetEase(openEase).SetUpdate(true);
        // SetUpdate(true) sorgt dafür, dass die Animation auch bei pausiertem Spiel (Time.timeScale = 0) läuft.
    }

    public void Close()
    {
        // 1. Auf 0 skalieren
        transform.DOScale(Vector3.zero, duration)
            .SetEase(closeEase)
            .SetUpdate(true)
            .OnComplete(() => {
                // 2. Erst wenn die Animation fertig ist: Deaktivieren
                gameObject.SetActive(false);
            });
    }
}