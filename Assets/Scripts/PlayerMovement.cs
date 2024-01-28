using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private float dirX = 0f;

    [SerializeField] private float moveSpeed = 9f;
    [SerializeField] private float jumpForce = 25f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {   
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump"))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector3(0, jumpForce, 0);
        }

        UpdateAnimationState();
    }

    void UpdateAnimationState()
    {
        if (dirX > 0f) {
            anim.SetBool("running", true);
            sprite.flipX = false;
        }
        else if (dirX < 0f) {
            anim.SetBool("running", true);
            sprite.flipX = true;
        }
        else {
            anim.SetBool("running", false);
        }
    }
}
