using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public bool isMoving = false; // to be toggled by external scripts
    [HideInInspector]
    public Vector3 velocity; // to be exposed to the player movement script
    
    [SerializeField]
    private Transform startPoint, endPoint, targetPoint;
    [SerializeField]
    private float m_speed = 1f;
    private bool m_isReversing = false;
    private Rigidbody m_Rigidbody;
    private Vector3 m_lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPoint.position;
        velocity = new Vector3(0f, 0f, 0f);
        m_lastPosition = transform.position;
        targetPoint = endPoint;
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update() 
    {
        velocity = (transform.position - m_lastPosition) / Time.deltaTime;
        m_lastPosition = transform.position;
    }

    // Apply physics-based movement in FixedUpdate
    void FixedUpdate()
    {
        // checkCollisions();
        if (isMoving)
        {
            Vector3 platformVelocity = targetPoint.position - transform.position;

            m_Rigidbody.MovePosition(transform.position + platformVelocity.normalized * Time.deltaTime * m_speed);

            // Reverse the platform if it's reached either end
            if ((m_Rigidbody.position - targetPoint.position).sqrMagnitude < 0.001f)
            {
                if (m_isReversing)
                {
                    targetPoint = endPoint;
                    m_isReversing = false;
                }
                else
                {
                    targetPoint = startPoint;
                    m_isReversing = true;
                }
            }
        }
    }

    /**
     * Checks if the player is on this platform using OverlapBox
     */
    // void checkCollisions()
    // {
    //     // Use the OverlapBox to detect if there are any other colliders within this box area.
    //     Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position + new Vector3(0f, 1.0f, 0f), new Vector3(transform.localScale.x, 2.0f, transform.localScale.z), Quaternion.identity);

    //     // Check when the player is coming into contact with the box
    //     int i = 0;
    //     while (i < hitColliders.Length)
    //     {
    //         // Debug.Log("Hit: " + hitColliders[i].gameObject.name);
    //         if (hitColliders[i].gameObject.tag == "Player")
    //         {
    //             Debug.Log("Player is on the platform");
    //             hitColliders[i].transform.parent = transform;
    //             return;
    //         }
    //         i++;
    //     }

    //     // Debug.Log("Player is not on the platform");
    //     transform.DetachChildren();
    // }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player is on the platform");
            isMoving = true;
            collision.transform.parent = transform;

            // Change scene after 3 seconds
            StartCoroutine(GoToBioLab());
        }
    }

    /**
     * Draws a red cube around the platform to show the OverlapBox 
     * for testing purposes
     */
    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(transform.position + new Vector3(0f, 1.0f, 0f), new Vector3(transform.localScale.x, 2.0f, transform.localScale.z));
    // }

    IEnumerator GoToBioLab()
    {
        yield return new WaitForSeconds(3);
        isMoving = true;
        GameObject.Find("GameManager").GetComponent<GameManager>().SetCurrentLevel(Level.biolab);
    }
}
