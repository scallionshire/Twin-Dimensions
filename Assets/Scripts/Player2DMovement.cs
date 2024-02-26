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

        if (Input.GetKey("space"))
        {
            movement = movement * 0.7f; // TODO: find some way to slow down walking anim?

            // Interact with whatever block we last collided into
            if (collidedBlock != null && collidedBlock.tag == "Block") {
                collidedBlock.transform.parent = transform;
            } else if (collidedBlock != null && collidedBlock.tag == "Extrudable") {
                collidedBlock.GetComponent<Extrudable>().Extrude();
            }
        } else {
            // Remove the currently held block from being a child of the player
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
        if (transform.childCount < 2 || collision.gameObject.tag == "Extrudable")
        {
            collidedBlock = collision.gameObject;
        }
    }
}

