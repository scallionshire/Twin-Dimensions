using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScreen : MonoBehaviour
{
    public Material fadeMaterial;

    private List<Renderer> renderers = new List<Renderer>();

    // Start is called before the first frame update
    void Start()
    {
        // Get all renderers in this object and its children
        renderers.AddRange(GetComponentsInChildren<Renderer>());
    }

    public void TurnOnScreen()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material = fadeMaterial;
        }
    }
}
