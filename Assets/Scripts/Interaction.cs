using UnityEditor;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public Camera playerCamera; 
    public float interactDistance = 5f;
    public LayerMask layers;
    public bool prevHit = false;
    public GameObject prevInteractable;

    private DialogueManager dialogueManager;
    private PlayerFader playerFader;
    private TooltipManager tooltipManager;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main; 
        }

        dialogueManager = GameObject.Find("GameManager").GetComponent<DialogueManager>();
        playerFader = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerFader>();
        tooltipManager = GameObject.Find("TooltipCanvas").GetComponent<TooltipManager>();
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
                if (tooltipManager != null && !dialogueManager.dialogueActive)
                {
                    tooltipManager.ToggleClickTooltip(true);
                } else if (dialogueManager.dialogueActive) {
                    tooltipManager.ToggleClickTooltip(false);
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
            } else {
                // Handle case where object is destroyed
                playerFader.ResetFade();
            }

            if (tooltipManager != null)
            {
                tooltipManager.GetComponent<TooltipManager>().ToggleClickTooltip(false);
            }
        } else {
            prevHit = false;
        }
    
    }
}
