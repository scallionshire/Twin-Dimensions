using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider collision)
    {
        transform.parent.GetComponent<CubeMonitorDetectionTrigger>().OnChildTriggerEntered(collision);
    }

    void OnTriggerExit(Collider collision)
    {
        transform.parent.GetComponent<CubeMonitorDetectionTrigger>().OnChildTriggerExited(collision);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        transform.parent.GetComponent<Player2DMovement>().OnChildTriggerEntered2D(collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        transform.parent.GetComponent<Player2DMovement>().OnChildTriggerExited2D(collision);
    }
}
