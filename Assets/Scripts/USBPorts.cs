using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class USBPorts : MonoBehaviour
{
    public int puzzleId;
    public string eventName = "event:/SFX3D/USBInsert";
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void PlugInUSB()
    {
        RuntimeManager.PlayOneShot(eventName, transform.position);
        if (gameManager.gameState.USBInserted) {
            gameManager.SetCurrentPuzzle(puzzleId);
            gameManager.SwitchToPuzzle(puzzleId);
        }
    }
}
