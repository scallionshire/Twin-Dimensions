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
        // gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {   
                interactable.LookAt();
                if (Input.GetKeyDown("e")) // "e" is the key to interact with objects
                {   
                    interactable.Interact();
                }
            }

            // if (Input.GetKeyDown("e")) // "e" is the key to interact with objects
            // {   
                
            //     if (interactable != null && gameManager.gameState.PlayerHasUSB && !gameManager.gameState.USBInserted)
            //     {   
            //         // audioManager.Play("USB");
            //         interactable.Interact();
            //         gameManager.gameState.USBInserted = true;
            //     }
            //     else if (interactable != null)
            //     {   
            //         //audioManager.Play("Pickup");
            //         interactable.Interact();
            //         gameManager.gameState.PlayerHasUSB = true;
            //     }
            // }
        }
    
    }
}
