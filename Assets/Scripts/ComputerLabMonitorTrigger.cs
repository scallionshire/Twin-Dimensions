using UnityEngine;

public class ComputerLabMonitorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("The player has entered the detection zone.");
        }
    }
}