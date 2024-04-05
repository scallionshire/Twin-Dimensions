using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
    private CinemachineBrain cinemachineBrain;

    private Collider[] hitColliders =  new Collider[3];
    private RaycastHit rayHit;
    private bool blockingInteractable = false;
    private int numColliders;

    void Start()
    {
        playerFader = GetComponent<PlayerFader>();
        dialogueManager = GameObject.Find("GameManager").GetComponent<DialogueManager>();
        tooltipManager = GameObject.Find("TooltipCanvas").GetComponent<TooltipManager>();
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
    }

    void Update()
    {   
        if (playerFader == null) Debug.Log("omfg");
        bool currentlyBlockingInteractable;

        // Don't need to worry about checking current camera since this component is disabled when a cutscene is happening
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 9f, Color.green);
        
        currentlyBlockingInteractable = Physics.Raycast(ray, out rayHit, 9f, layers);
        
        numColliders = Physics.OverlapSphereNonAlloc(interactionCenter.position, interactRadius, hitColliders, layers);
        
        if (currentlyBlockingInteractable && !blockingInteractable)
        {
            playerFader.Fade();
            blockingInteractable = currentlyBlockingInteractable;
        } else if (!currentlyBlockingInteractable && blockingInteractable)
        {
            playerFader.ResetFade();
            blockingInteractable = currentlyBlockingInteractable;
        }

        if (numColliders > 0)
        {
            Interactable interactable;

            // If there's more than one nearby object, sort by distance and choose the closest one
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
 
            // If the object has an interactable component
            if (interactable != null)
            {   
                if (tooltipManager != null && !dialogueManager.dialogueActive)
                {
                    tooltipManager.ToggleClickTooltip(true);
                } else if (dialogueManager.dialogueActive) {
                    tooltipManager.ToggleClickTooltip(false);
                }
                
                prevHit = true;

                if (Input.GetButtonDown("Fire1")) // click/b to interact
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
