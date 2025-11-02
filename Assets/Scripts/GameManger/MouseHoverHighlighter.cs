using UnityEngine;

public class MouseHoverHighlighter : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer; // optional
    private GameObject lastHitObject;
    private Material originalMat;
    private Renderer lastRenderer;

    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float emissionIntensity = 1.5f;

    void Update()
    {
        // Ray von der Maus in die Szene (Kamera ? Mausposition)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // optional: begrenzen auf bestimmte Layer
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Tower"))
            {
                if (hitObject != lastHitObject)
                {
                    ClearHighlight();

                    lastHitObject = hitObject;
                    lastRenderer = hitObject.GetComponent<Renderer>();

                    if (lastRenderer != null)
                    {
                        originalMat = lastRenderer.material;
                        Highlight(lastRenderer);
                    }
                }
            }
            
        }
        else
        {
            ClearHighlight();
        }
    }

    void Highlight(Renderer rend)
    {
        Debug.Log("HoverIn");
        if (rend.material.HasProperty("_EmissionColor"))
        {
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", highlightColor * emissionIntensity);
        }
        else
        {
            rend.material.color = highlightColor;
        }
    }

    void ClearHighlight()
    {
        if (lastRenderer != null)
        {
            if (originalMat != null && originalMat.HasProperty("_EmissionColor"))
            {
                lastRenderer.material.DisableKeyword("_EMISSION");
                lastRenderer.material.SetColor("_EmissionColor", Color.black);
            }
            else if (originalMat != null)
            {
                lastRenderer.material.color = originalMat.color;
            }
        }

        lastHitObject = null;
        lastRenderer = null;
    }
}
