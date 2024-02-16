using UnityEngine;

public class Interaction : MonoBehaviour
{
    public Camera playerCamera; 
    public float interactDistance = 5f;
    public LayerMask layers;
    private GameManager gameManager;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main; 
        }
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {   
        // Constantly detect ray
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactDistance, layers))
        {   
            Debug.DrawLine(ray.origin, hit.point, Color.red);
            Debug.Log(hit.collider.name);
            InteractablePC interactablePC = hit.collider.GetComponent<InteractablePC>();
            InteractableUSB interactableUSB = hit.collider.GetComponent<InteractableUSB>();

            if (interactablePC != null)
            {   
                interactablePC.LookAt();
            }
            else if (interactableUSB != null)
            {   
                interactableUSB.LookAt();
            }

            if (Input.GetKeyDown("e")) // "e" is the key to interact with objects
            {   
                if (interactablePC != null && gameManager.gameState.PlayerHasUSB && !gameManager.gameState.USBInserted)
                {   
                    // audioManager.Play("USB");
                    interactablePC.Interact();
                    gameManager.gameState.USBInserted = true;
                }
                else if (interactableUSB != null)
                {   
                    //audioManager.Play("Pickup");
                    interactableUSB.Interact();
                    gameManager.gameState.PlayerHasUSB = true;
                }
            }
        }
    
    }
}
