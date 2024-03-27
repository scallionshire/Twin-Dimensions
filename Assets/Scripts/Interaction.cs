using UnityEditor;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public Transform interactionCenter;
    public float interactRadius = 5f;
    public LayerMask layers;
    public bool prevHit = false;
    public GameObject prevInteractable;

    private DialogueManager dialogueManager;
    private PlayerFader playerFader;
    private TooltipManager tooltipManager;

    private Collider[] hitColliders =  new Collider[3];
    private int numColliders;

    void Start()
    {
        dialogueManager = GameObject.Find("GameManager").GetComponent<DialogueManager>();
        playerFader = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerFader>();
        tooltipManager = GameObject.Find("TooltipCanvas").GetComponent<TooltipManager>();
    }

    void Update()
    {   
        numColliders = Physics.OverlapSphereNonAlloc(interactionCenter.position, interactRadius, hitColliders, layers);

        if (numColliders > 0)
        {
            Interactable interactable;

            if (numColliders > 1)
            {
                float minDist = Mathf.Infinity;
                Collider closest = null;
                foreach (Collider c in hitColliders)
                {
                    if (c == null) continue;
                    float dist = (interactionCenter.position - c.transform.position).sqrMagnitude;
                    if (dist < minDist)
                    {
                        closest = c;
                        minDist = dist;
                    }
                }
                interactable = closest.GetComponent<Interactable>();
            } else {
                interactable = hitColliders[0].GetComponent<Interactable>();
            }

            if (interactable != null)
            {   
                if (tooltipManager != null && !dialogueManager.dialogueActive)
                {
                    tooltipManager.ToggleClickTooltip(true);
                } else if (dialogueManager.dialogueActive) {
                    tooltipManager.ToggleClickTooltip(false);
                }
                
                prevHit = true;
    
                if (Input.GetMouseButtonDown(0)) // "e" is the key to interact with objects
                {   
                    interactable.Interact();
                    interactable.RemoveHighlight();
                } else if (prevInteractable != interactable.gameObject) {
                    if (prevInteractable != null)
                    {
                        prevInteractable.GetComponent<Interactable>().RemoveHighlight();
                    }
                    interactable.Highlight();
                }

                prevInteractable = interactable.gameObject;
            }
        } else if (prevHit == true)
        {
            // If we're no longer interacting with anything
            prevHit = false;
            
            if (prevInteractable != null)
            {
                prevInteractable.GetComponent<Interactable>().RemoveHighlight();
                prevInteractable = null;
            } else {
                // Handle case where object is destroyed
                playerFader.ResetFade();
            }

            if (tooltipManager != null)
            {
                tooltipManager.GetComponent<TooltipManager>().ToggleClickTooltip(false);
            }
        } else {
            // We were never interacting with anything
            prevHit = false;
        }
    }

    // Draw the sphere in the scene view
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(interactionCenter.position, interactRadius);
    }
}
