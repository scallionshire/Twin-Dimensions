using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{   
    public bool is2DInteractable = false;

    public UnityEvent interactionEvent;
    public Material highlightMaterial;
    [HideInInspector]
    public bool wasHighlighted = false;

    private List<Material> originalMaterials = new List<Material>();
    private List<Renderer> renderers = new List<Renderer>();
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        if (!is2DInteractable)
        {
            // Get all renderers in this object and its children
            renderers.AddRange(GetComponentsInChildren<Renderer>());

            // Store the original materials for each renderer
            foreach (Renderer rend in renderers)
            {
                originalMaterials.Add(rend.material);
            }
        } else {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void Highlight()
    {
        foreach (Renderer rend in renderers)
        {
            rend.material = highlightMaterial;
        }
        wasHighlighted = true;
    }

    public void RemoveHighlight()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            if (renderers[i] != null) // Check if the renderer still exists
            {
                renderers[i].material = originalMaterials[i];
            }
        }
    }

    public void Interact()
    {
        interactionEvent.Invoke();
    }
}
