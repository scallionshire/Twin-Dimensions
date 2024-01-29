using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public bool isMoving = false;
    [SerializeField]
    private Transform startPoint, endPoint, targetPoint;
    [SerializeField]
    private float m_Speed = 1f;
    private bool m_isReversing = false;
    private Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPoint.position;
        targetPoint = endPoint;
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void MyCollisions()
    {
        // Use the OverlapBox to detect if there are any other colliders within this box area.
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position + new Vector3(0f, 1.0f, 0f), new Vector3(2.0f, 2.0f, 2.0f), Quaternion.identity);
        int i = 0;
        // Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            // Output all of the collider names
            Debug.Log("Hit : " + hitColliders[i].name + i);
            // Increase the number of Colliders in the array
            if (hitColliders[i].gameObject.tag == "Player")
            {
                Debug.Log("Player is on the platform");
                hitColliders[i].transform.parent = transform;
            }
            i++;
        }
        if (hitColliders.Length < 2)
        {
            Debug.Log("Player is not on the platform");
            transform.DetachChildren();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireCube(transform.position + new Vector3(0f, 1.0f, 0f), new Vector3(2.0f, 2.0f, 2.0f));
    }

    void FixedUpdate()
    {
        MyCollisions();
        if (isMoving)
        {
            //Store user input as a movement vector
            Vector3 velocity = targetPoint.position - transform.position;

            //Apply the movement vector to the current position, which is
            //multiplied by deltaTime and speed for a smooth MovePosition
            m_Rigidbody.MovePosition(transform.position + velocity.normalized * Time.deltaTime * m_Speed);

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
}
