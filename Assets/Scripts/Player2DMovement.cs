using UnityEngine;

public class Player2DMovement : MonoBehaviour
{
    private Animator _animator;

    public float moveSpeed = 2.5f;
    // private float moveLimiter = 0.7f;
    private Vector2 movement;
    private bool movingBlock = false;
    public Rigidbody2D rb;
    private AudioManager audioManager;

    void Start()
    {
        _animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();

        audioManager = GameObject.Find("GameManager").GetComponent<AudioManager>();
    }

    void Update()
    {
        // Gives a value between -1 and 1
        movement.x = Input.GetAxisRaw("Horizontal"); // -1 is left
        movement.y = Input.GetAxisRaw("Vertical"); // -1 is down

        if (Input.GetKey("space"))
        {
            movingBlock = true;
            movement = movement * 0.5f;
        } else {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).transform.parent = GameObject.Find("Tilemap_Blocks").transform;
            }
            if (audioManager.isPlaying("BoxSliding"))
            {
                audioManager.Pause("BoxSliding");
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
        if (movingBlock && collision.gameObject.tag == "Block" && transform.childCount < 1)
        {
            if (!audioManager.isPlaying("BoxSliding"))
            {
                audioManager.Play("BoxSliding");
            }
            collision.gameObject.transform.parent = transform;
        }
    }
}

