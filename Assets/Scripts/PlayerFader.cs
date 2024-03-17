using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFader : MonoBehaviour
{
    public float fadeSpeed = 1f;
    [Range(0, 1)]
    public float fadeAmount;
    public Material fadeMaterial;
    public bool isFaded = false;

    private List<Material> originalMaterials = new List<Material>();
    private List<Color> originalColors = new List<Color>();
    private List<Renderer> renderers = new List<Renderer>();

    // Start is called before the first frame update
    void Start()
    {
        // Get all renderers in this object and its children
        renderers.AddRange(GetComponentsInChildren<Renderer>());

        // Store the original materials for each renderer
        foreach (Renderer rend in renderers)
        {
            originalMaterials.Add(rend.material);
            originalColors.Add(rend.material.color);
        }
    }

    public void Fade()
    {
        // Debug.Log("Fading");
        isFaded = true;
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material = fadeMaterial;
        }
        // StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        bool stop = false;
        while (!stop && isFaded)
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                // Old alpha-based fade

                // Color currentColor = renderers[i].material.color;
                // Color targetColor = new Color(Mathf.Max(originalColors[i].r - fadeAmount, 0f), Mathf.Max(originalColors[i].g - fadeAmount, 0f), Mathf.Max(originalColors[i].b - fadeAmount, 0f), fadeAmount);

                // if (Mathf.Abs(currentColor.a - fadeAmount) < 0.01f)
                // {
                //     renderers[i].material.color = targetColor;
                //     stop = true;
                // } else {
                    
                //     Color smoothColor = Color.Lerp(currentColor, targetColor, fadeSpeed * Time.deltaTime);
                //     renderers[i].material.color = smoothColor;
                // }
            }
            yield return null;
        }
    }

    public void ResetFade()
    {
        // Debug.Log("Resetting fade");
        isFaded = false;
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material = originalMaterials[i];
        }
        // StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        bool stop = false;
        while (!stop && !isFaded)
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                Color currentColor = renderers[i].material.color;
                Color targetColor = new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b, 1f);

                if (Mathf.Abs(1f - currentColor.a) < 0.01f)
                {
                    renderers[i].material.color = targetColor;
                    stop = true;
                } else {
                    Color smoothColor = Color.Lerp(currentColor, targetColor, fadeSpeed * Time.deltaTime);
                    renderers[i].material.color = smoothColor;
                }
            }
            yield return null;
        }
    }
}
