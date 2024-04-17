using UnityEngine;
using FMODUnity;

public class Player2DMovement : MonoBehaviour
{
    public float moveSpeed = 2.5f;

    [HideInInspector]
    public Vector2 movement;
    private Vector2 lastMovement;
    private Animator _animator;
    private Rigidbody2D rb;
    private FMOD.Studio.EventInstance footstepsSound;

    void Start()
    {
        _animator = GetComponent<Animator>();

        footstepsSound = RuntimeManager.CreateInstance("event:/SFX2D/2DFootsteps");

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Gives a value between -1 and 1
        if (movement.sqrMagnitude > 0)
        {
            lastMovement = movement;
        }

        movement.x = Input.GetAxisRaw("Horizontal"); // -1 is left
        movement.y = Input.GetAxisRaw("Vertical"); // -1 is down

        HandleFootstepsSound();

        _animator.SetFloat("Horizontal", movement.x);
        _animator.SetFloat("Vertical", movement.y);
        _animator.SetFloat("LastHorizontal", lastMovement.x);
        _animator.SetFloat("LastVertical", lastMovement.y);
        _animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void OnDisable()
    {
        footstepsSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
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
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}

