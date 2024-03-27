using UnityEngine;
using FMODUnity;
using System.Collections;

public class CubeMonitorDetectionTrigger : MonoBehaviour
{
    public GameObject monitorCube; 
    private bool isMonitoringActive = true;
    public string eventName = "event:/SFX3D/MonitorBeep";

    public void OnChildTriggerEntered(Collider other)
    {
        if (other.CompareTag("Extrudable"))
        {   
            isMonitoringActive = false;
            SetMonitorCubeActive(false); 
            RuntimeManager.PlayOneShot(eventName, transform.position);
            Debug.Log("Monitoring blocked by an extrudable object.");
        }
        if (isMonitoringActive && other.CompareTag("Player"))
        {
            Debug.Log("The player has entered the detection zone.");
            ScreenFade fadeEffect = FindObjectOfType<ScreenFade>(); 
            if (fadeEffect != null)
            {
                fadeEffect.TriggerCaughtEffect(); 
                StartCoroutine(WaitToFadeBack(fadeEffect));
            }
        }
    }

    IEnumerator WaitToFadeBack(ScreenFade fadeEffect)
    {
        yield return new WaitForSeconds(2f); 
        fadeEffect.TriggerReturnEffect();
        GetComponent<ResetToCheckpoint>().Reset();
    }

    public void OnChildTriggerExited(Collider other)
    {
        if (other.CompareTag("Extrudable"))
        {
            Debug.Log("hi im exiting");
            isMonitoringActive = true;
            SetMonitorCubeActive(true); 
            RuntimeManager.PlayOneShot(eventName, transform.position);
            Debug.Log("Monitoring unblocked.");
        }
    }

    private void SetMonitorCubeActive(bool isActive)
    {
        if (monitorCube != null)
        {
            foreach (Transform child in transform) {
                child.gameObject.SetActive(isActive);
            }
        }
    }
}