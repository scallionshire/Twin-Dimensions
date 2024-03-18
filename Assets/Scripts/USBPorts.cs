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
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void PlugInUSB()
    {   
        Debug.Log("USB inserted");
        RuntimeManager.PlayOneShot(eventName, transform.position);
        if (gameManager.gameState.USBInserted) {
            if (isPuzzlePort) {
                Debug.Log("Switching to puzzle " + id);
                gameManager.SwitchToPuzzle(id, level);
            } else {
                gameManager.SwitchToMap(id, level);
            }
        }
    }
}
