using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionInteractable : MonoBehaviour
{
    public UnityEvent interactionEvent;

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            interactionEvent.Invoke();
        }
    }
}
