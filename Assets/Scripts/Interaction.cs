using UnityEngine;

public class Interaction : MonoBehaviour
{
    public Camera playerCamera; 
    public float interactDistance = 5f;
    public LayerMask layers;
    public bool prevHit = false;
    public GameObject prevInteractable;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main; 
        }
    }

    void Update()
    {   
        // Constantly detect ray
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactDistance, layers))
        {   
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {   
                if (GameObject.Find("TooltipCanvas") != null)
                {
                    GameObject.Find("TooltipCanvas").GetComponent<TooltipManager>().ShowClickTooltip();
                }
                prevHit = true;
                prevInteractable = hit.collider.gameObject;
                interactable.Highlight();
                if (Input.GetMouseButtonDown(0)) // "e" is the key to interact with objects
                {   
                    interactable.Interact();
                }
            }
        } else if (prevHit == true)
        {
            // Debug.Log("Removing highlight");
            prevHit = false;
            if (prevInteractable != null)
            {
                prevInteractable.GetComponent<Interactable>().RemoveHighlight();
            }
        } else {
            prevHit = false;
        }
    
    }
}
