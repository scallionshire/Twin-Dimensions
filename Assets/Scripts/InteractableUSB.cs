using UnityEngine;
using System.Collections.Generic;

public class InteractableUSB : MonoBehaviour
{   
    public Material highlightMaterial;
    private List<Material> originalMaterials = new List<Material>();
    private List<Renderer> renderers = new List<Renderer>();
    
    private bool wasHighlighted = false;

    void Start()
    {
        // Get all renderers in this object and its children
        renderers.AddRange(GetComponentsInChildren<Renderer>());

        // Store the original materials for each renderer
        foreach (Renderer rend in renderers)
        {
            originalMaterials.Add(rend.material);
        }
    }

    void Update()
    {
        if (!wasHighlighted)
        {
            RemoveHighlight();
        }
        else 
        {
            wasHighlighted = false;
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
        Debug.Log("Interacted with " + gameObject.name);
        gameObject.SetActive(false);
    }

    public void LookAt()
    {
        Highlight();
    }
}
