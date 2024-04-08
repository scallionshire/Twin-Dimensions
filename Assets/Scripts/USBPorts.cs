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
    private bool alreadyInserted = false;

    public void PlugInUSB()
    {   
        if (GameManager.instance.gameState.USBInserted) {
            RuntimeManager.PlayOneShot(eventName, transform.position);
            if (isPuzzlePort) {
                GameManager.instance.SwitchToPuzzle(id, level, !alreadyInserted);
            } else {
                GameManager.instance.SwitchToMap(id, level, !alreadyInserted);
            }
            alreadyInserted = true;
        }
    }
}
