using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;

public class MouseHoverHighlighter : MonoBehaviour
{
    private GameObject lastHitObject;
    private List<Renderer> lastRenderers = new List<Renderer>();

    private List<Color> originalColors = new List<Color>();
    private List<Color> originalEmissions = new List<Color>();



    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Tower"))
            {
                if (hitObject != lastHitObject)
                {
                    MouseExits();      // alten Hover entfernen
                    MouseEnters(hitObject); // neuen Hover aktivieren
                    lastHitObject = hitObject;
                }
                return; // wichtig
            }
        }

        // nur wenn wir vorher auf einem Tower waren → Hover beenden
        if (lastHitObject != null)
        {
            MouseExits();
            lastHitObject = null;
        }
    }


    private void MouseEnters(GameObject obj)
    {
        lastRenderers = new List<Renderer>(obj.GetComponentsInChildren<Renderer>());
        originalColors.Clear();
        originalEmissions.Clear();

        foreach (var rend in lastRenderers)
        {
            Material mat = rend.material;

            if (mat.HasProperty("_Color"))
                originalColors.Add(mat.color);
            else
                originalColors.Add(Color.white);

            if (mat.HasProperty("_EmissionColor"))
            {
                originalEmissions.Add(mat.GetColor("_EmissionColor"));
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", mat.color * 1.5f); // 50% heller
            }
        }
    }
    Color Brighten(Color c, float factor)
    {
        return new Color(
            Mathf.Clamp01(c.r * factor),
            Mathf.Clamp01(c.g * factor),
            Mathf.Clamp01(c.b * factor),
            c.a
        );
    }


    private void MouseExits()
    {
        if (lastRenderers.Count == 0) return;

        for (int i = 0; i < lastRenderers.Count; i++)
        {
            Material mat = lastRenderers[i].material;

            if (mat.HasProperty("_EmissionColor"))
                mat.SetColor("_EmissionColor", originalEmissions[i]);

            if (mat.HasProperty("_Color"))
                mat.color = originalColors[i];
        }

        lastRenderers.Clear();
    }
}

