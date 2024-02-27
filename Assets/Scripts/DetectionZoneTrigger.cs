using UnityEngine;

public class DetectionZoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("The player has entered the detection zone.");
        }
    }
}