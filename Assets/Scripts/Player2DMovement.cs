using UnityEngine;

public class Player2DMovement : MonoBehaviour
{
    public float moveSpeed = 2.5f;

    private Vector2 movement;
    private Animator _animator;
    private Rigidbody2D rb;

    [HideInInspector]
    public GameObject collidedBlock;

    void Start()
    {
        _animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Gives a value between -1 and 1
        movement.x = Input.GetAxisRaw("Horizontal"); // -1 is left
        movement.y = Input.GetAxisRaw("Vertical"); // -1 is down

        // If player is pressing space and they aren't currently holding a block
        if (Input.GetKey("space") && transform.childCount < 2)
        {
            movement = movement * 0.7f; // TODO: find some way to slow down walking anim?

            // Interact with whatever block we last collided into
            if (collidedBlock != null && collidedBlock.tag == "Block") {
                collidedBlock.transform.parent = transform;
            } else if (collidedBlock != null && collidedBlock.tag == "Extrudable") {
                collidedBlock.GetComponent<Extrudable>().Extrude();
            }

            collidedBlock = null;
        } else if (Input.GetKey("space") && transform.childCount >= 2) {
            // We don't care about other collisions while we're already holding a block
            collidedBlock = null;
        } else if (!Input.GetKey("space") && transform.childCount >= 2) {
            // Remove the currently held block from being a child of the player once player lets go of space key
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).tag == "Block") {
                    transform.GetChild(i).transform.parent = GameObject.Find("Blocks").transform;
                }
            }
        }

        _animator.SetFloat("Horizontal", movement.x);
        _animator.SetFloat("Vertical", movement.y);
        _animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        if (collision.gameObject.tag == "Block" || collision.gameObject.tag == "Extrudable")
        {
            Debug.Log("Collided with block: " + collision.gameObject.name);
            collidedBlock = collision.gameObject;
        }
    }
}

