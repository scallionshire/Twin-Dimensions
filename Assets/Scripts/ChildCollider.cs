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
}
