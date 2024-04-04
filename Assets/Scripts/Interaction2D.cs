using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Interaction2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private TooltipManager tooltipManager;
    private FMOD.Studio.EventInstance blockMovingSound;
    private Player2DMovement player2DMovement;
    private Interactable currentInteractable;
    [HideInInspector]
    public GameObject collidedBlock;

    // Start is called before the first frame update
    void Start()
    {
        blockMovingSound = RuntimeManager.CreateInstance("event:/SFX3D/BoxPushV2");
        rb = GetComponent<Rigidbody2D>();

        if (GameObject.Find("TooltipCanvas") != null)
        {
            tooltipManager = GameObject.Find("TooltipCanvas").GetComponent<TooltipManager>();
        }

        player2DMovement = GetComponent<Player2DMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        if (currentInteractable != null && Input.GetButtonDown("Fire1"))
        {
            currentInteractable.Interact();
        }   
    }

    void FixedUpdate()
    {
        // If player is pressing space and they aren't currently holding a block
        if (Input.GetButton("Drag") && collidedBlock != null)
        {
            player2DMovement.movement = player2DMovement.movement * 0.7f; // Slow down movement while holding a block

            if (blockMovingSound.getPlaybackState(out var playbackState) != FMOD.RESULT.OK || playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                blockMovingSound.start();
            }

            // Interact with whatever block we last collided into
            if (collidedBlock != null && collidedBlock.tag == "Block") {
                collidedBlock.GetComponent<Rigidbody2D>().MovePosition(collidedBlock.GetComponent<Rigidbody2D>().position + player2DMovement.movement * player2DMovement.moveSpeed * Time.fixedDeltaTime);
            } else if (collidedBlock != null && collidedBlock.tag == "Extrudable") {
                collidedBlock.GetComponent<Extrudable>().Extrude();
            }
        }
        else {
            blockMovingSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Extrudable")
        {
            if (tooltipManager != null)
            {
                tooltipManager.ToggleSpaceTooltip(true);
            }
            collidedBlock = collision.gameObject;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Extrudable")
        {
            collidedBlock = null;
            if (tooltipManager != null)
            {
                tooltipManager.ToggleSpaceTooltip(false);
            }
        }
    }

    public void OnChildTriggerEntered2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Block")
        {
            collidedBlock = collider.gameObject;
        }

        if (collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            currentInteractable = collider.gameObject.GetComponent<Interactable>();
            if (tooltipManager != null)
            {
                tooltipManager.ToggleClickTooltip(true);
            }
        }
    }

    public void OnChildTriggerExited2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Block")
        {
            collidedBlock = null;
        }
        if (collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            currentInteractable = null;
            if (tooltipManager != null)
            {
                tooltipManager.ToggleClickTooltip(false);
            }
        }
    }
}
