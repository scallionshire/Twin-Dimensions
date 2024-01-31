using UnityEngine;

public class Interaction : MonoBehaviour
{
    public Camera playerCamera; 
    public float interactDistance = 5f;
    public LayerMask layers;
    private GameManager gameManager;
    private AudioManager audioManager;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main; 
        }
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.Find("GameManager").GetComponent<AudioManager>();
    }

    void Update()
    {   
        // Constantly detect ray
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactDistance, layers))
        {   
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
                    audioManager.Play("USB");
                    interactablePC.Interact();
                    gameManager.gameState.USBInserted = true;
                }
                else if (interactableUSB != null)
                {   
                    audioManager.Play("Pickup");
                    interactableUSB.Interact();
                    gameManager.gameState.PlayerHasUSB = true;
                }
            }
        }
    
    }
}
