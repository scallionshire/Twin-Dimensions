using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.UI;
using TMPro;

public class USBPorts : MonoBehaviour
{
    // TODO: check if we're in the 3D world or the 2D world
    public GameStateCondition conditionToCheck = GameStateCondition.insertedUSB;
    private bool conditionMet = false;
    public bool isPuzzlePort = true; // if true, this port will trigger a puzzle via the 2D scene; otherwise, it will trigger the 2d room via the 3d scene
    public int id;
    public Level level;
    public string eventName = "event:/SFX3D/USBInsert";
    private bool alreadyInserted = false;

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
                GameManager.instance.SwitchToPuzzle(id, level, !alreadyInserted);
            } else {
                StartCoroutine(Hacking());
            }
            alreadyInserted = true;
        }
    }

    IEnumerator Hacking()
    {
        GameObject hackingCanvas = GameObject.Find("HackingCanvas");
        hackingCanvas.GetComponent<HackingSlider>().ToggleSlider(true);

        Slider slider = hackingCanvas.GetComponentInChildren<Slider>();
        TMP_Text text = hackingCanvas.GetComponentInChildren<TMP_Text>();

        float elapsedTime = 0;
        while (elapsedTime <= 2)
        {
            elapsedTime += Time.deltaTime;
            slider.value = elapsedTime / 2;
            yield return null;
        }
        
        hackingCanvas.GetComponent<HackingSlider>().ToggleSlider(false);
        GameManager.instance.SwitchToMap(level, !alreadyInserted);
    }
}
