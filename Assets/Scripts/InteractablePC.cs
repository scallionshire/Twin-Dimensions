using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InteractablePC : MonoBehaviour
{   
    public Material highlightMaterial;
    private List<Material> originalMaterials = new List<Material>();
    private List<Renderer> renderers = new List<Renderer>();
    public string sceneToLoad = "TopDownEnv"; 

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
        RemoveHighlight();

        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject.Find("Door").SetActive(false);
        }
    }

    public void Highlight()
    {
        foreach (Renderer rend in renderers)
        {
            rend.material = highlightMaterial;
        }
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
        // Implement your interaction logic here
        Debug.Log("Interacted with " + gameObject.name);
        // gameObject.SetActive(false);
        // Save player position using GameManager
        GameObject.Find("Player").GetComponent<PlayerManager>().savePlayer();
        SceneManager.LoadScene(sceneToLoad);

    }

    public void LookAt()
    {
        Highlight();
    }
}
