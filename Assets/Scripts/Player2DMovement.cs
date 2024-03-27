using UnityEngine;
using FMODUnity;

public class Player2DMovement : MonoBehaviour
{
    public float moveSpeed = 2.5f;

    private Vector2 movement;
    private Animator _animator;
    private Rigidbody2D rb;
    private TooltipManager tooltipManager;
    private FMOD.Studio.EventInstance footstepsSound;
    private FMOD.Studio.EventInstance blockMovingSound;

    [HideInInspector]
    public GameObject collidedBlock;

    void Start()
    {
        _animator = GetComponent<Animator>();

        footstepsSound = RuntimeManager.CreateInstance("event:/SFX2D/2DFootsteps");
        blockMovingSound = RuntimeManager.CreateInstance("event:/SFX3D/BoxPushV2");

        rb = GetComponent<Rigidbody2D>();

        if (GameObject.Find("TooltipCanvas") != null)
        {
            tooltipManager = GameObject.Find("TooltipCanvas").GetComponent<TooltipManager>();
        }
    }

    void Update()
    {
        // Gives a value between -1 and 1
        movement.x = Input.GetAxisRaw("Horizontal"); // -1 is left
        movement.y = Input.GetAxisRaw("Vertical"); // -1 is down

        HandleFootstepsSound();

        _animator.SetFloat("Horizontal", movement.x);
        _animator.SetFloat("Vertical", movement.y);
        _animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void HandleFootstepsSound()
    {
        if (movement.sqrMagnitude > 0)
        {
            if (footstepsSound.getPlaybackState(out var playbackState) != FMOD.RESULT.OK || playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                footstepsSound.start(); 
            }
        }
        else
        {
            footstepsSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); 
        }
    }

    void FixedUpdate()
    {
        // If player is pressing space and they aren't currently holding a block

        if (Input.GetButton("Drag")&& collidedBlock != null)
        {
            movement = movement * 0.7f; // Slow down movement while holding a block

            if (blockMovingSound.getPlaybackState(out var playbackState) != FMOD.RESULT.OK || playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                blockMovingSound.start();
            }

            // Interact with whatever block we last collided into
            if (collidedBlock != null && collidedBlock.tag == "Block") {
                Vector3 distance = transform.position - collidedBlock.transform.position;
                collidedBlock.GetComponent<Rigidbody2D>().MovePosition(collidedBlock.GetComponent<Rigidbody2D>().position + movement * moveSpeed * Time.fixedDeltaTime);
            } else if (collidedBlock != null && collidedBlock.tag == "Extrudable") {
                collidedBlock.GetComponent<Extrudable>().Extrude();
            }
        }
        else {
            blockMovingSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        if (collision.gameObject.tag == "Block" || collision.gameObject.tag == "Extrudable")
        {
            if (tooltipManager != null)
            {
                tooltipManager.ToggleSpaceTooltip(true);
            }
            Debug.Log("Collided with block: " + collision.gameObject.name);
            collidedBlock = collision.gameObject;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Left collision with: " + collision.gameObject.name);
        if (collision.gameObject.tag == "Block" || collision.gameObject.tag == "Extrudable")
        {
            collidedBlock = null;
            if (tooltipManager != null)
            {
                tooltipManager.ToggleSpaceTooltip(false);
            }
        }
    }
}

