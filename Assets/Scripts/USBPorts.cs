using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class USBPorts : MonoBehaviour
{
    public bool isPuzzlePort = true; // if true, this port will trigger a puzzle; otherwise, trigger the map with extrudables
    public int id;
    public Level level;
    public string eventName = "event:/SFX3D/USBInsert";

    public void PlugInUSB()
    {   
        Debug.Log("USB inserted");
        RuntimeManager.PlayOneShot(eventName, transform.position);
        if (GameManager.instance.gameState.USBInserted) {
            if (isPuzzlePort) {
                Debug.Log("Switching to puzzle " + id);
                GameManager.instance.SwitchToPuzzle(id, level);
            } else {
                GameManager.instance.SwitchToMap(id, level);
            }
        }
    }
}
