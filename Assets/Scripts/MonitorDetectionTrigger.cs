using UnityEngine;
public class MonitorDetectionTrigger : MonoBehaviour
{
    public Light[] pointLights;
    private bool isMonitoringActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Extrudable"))
        {   
            isMonitoringActive = false;
            SetPointLightsActive(false);
            Debug.Log("Monitoring blocked by an extrudable object.");
        }
        else if (isMonitoringActive && other.CompareTag("Player"))
        {
            Debug.Log("The player has entered the detection zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Extrudable"))
        {
            Debug.Log("hi im exiting");
            isMonitoringActive = true;
            SetPointLightsActive(true);
            Debug.Log("Monitoring unblocked.");
        }
    }

    private void SetPointLightsActive(bool isActive)
    {
        foreach (var light in pointLights)
        {
            light.enabled = isActive;
        }
    }
}