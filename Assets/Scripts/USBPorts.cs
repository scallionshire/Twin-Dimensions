using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class USBPorts : MonoBehaviour
{
    // TODO: check if we're in the 3D world or the 2D world
    public GameStateCondition conditionToCheck = GameStateCondition.insertedUSB;
    private bool conditionMet = false;

    public bool isPuzzlePort = true; // if true, this port will trigger a puzzle via the 2D scene; otherwise, it will trigger the 2d room via the 3d scene
    public int id;
    public Level level;
    public string eventName = "event:/SFX3D/USBInsert";

    public void PlugInUSB()
    {   
        switch (conditionToCheck)
        {
            case GameStateCondition.hasUSB:
                conditionMet = GameManager.instance.gameState.PlayerHasUSB;
                break;
            case GameStateCondition.insertedUSB:
                conditionMet = GameManager.instance.gameState.USBInserted;
                break;
            case GameStateCondition.hasBattery:
                conditionMet = GameManager.instance.gameState.BatteriesCollected > 0;
                break;
        }

        if (conditionMet) {
            RuntimeManager.PlayOneShot(eventName, transform.position);
            if (isPuzzlePort) {
                GameManager.instance.SwitchToPuzzle(id, level);
            } else {
                GameManager.instance.SwitchToMap(id, level);
            }
        }
    }
}
