using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USBPorts : MonoBehaviour
{
    public int puzzleId;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void PlugInUSB()
    {
        if (gameManager.gameState.USBInserted) {
            gameManager.SetCurrentPuzzle(puzzleId);
            gameManager.SwitchToPuzzle(puzzleId);
        }
    }
}
