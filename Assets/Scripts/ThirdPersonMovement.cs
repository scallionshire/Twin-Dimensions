using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public float jumpHeight = 2.0f;
    public float gravityValue = -9.81f;


    private Vector3 playerVelocity;
    private bool groundedPlayer;


    private Animator animator;
    // private AudioManager audioManager;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // audioManager = GameObject.Find("GameManager").GetComponent<AudioManager>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded; // Check if grounded
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal"); // A, D, Left, Right
        float vertical = Input.GetAxisRaw("Vertical"); // W, S, Up, Down
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
    
        animator.SetFloat("Speed", direction.sqrMagnitude);

        if (direction.magnitude >= 0.1f)
        {
            // Play footsteps
            // if (!audioManager.isPlaying("Walking"))
            // {
            //     audioManager.Play("Walking");
            // }

            

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            if (transform.parent != null) {
                Vector3 parentDir = transform.parent.gameObject.GetComponent<PlatformMovement>().velocity;
                controller.Move(moveDir.normalized * speed * Time.deltaTime + parentDir * Time.deltaTime);
            } else {
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }
        } else {
            // if (audioManager.isPlaying("Walking"))
            // {
            //     audioManager.Pause("Walking");
            // }
            if (transform.parent != null)
            {
                Vector3 parentDir = transform.parent.gameObject.GetComponent<PlatformMovement>().velocity;
                controller.Move(parentDir * Time.deltaTime);
            }
        }

        // Jump
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            Debug.Log("jumped");
            // animator.SetBool("Jump", true);
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime); // Apply gravity and jump velocity
    }
}
