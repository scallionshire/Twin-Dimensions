using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class USBPorts : MonoBehaviour
{
    public GameStateCondition conditionToCheck = GameStateCondition.insertedUSB;
    private bool conditionMet = false;
    [Tooltip("If isPuzzlePort = true, the port will trigger a puzzle via the 2D scene. Otherwise, it will trigger the 2D room via the 3D scene.")]
    public bool isPuzzlePort = true;
    public int id;
    public Level level;
    public string eventName = "event:/SFX3D/USBInsert";
    private bool alreadyInserted = false;

    public void PlugInUSB()
    {   
        conditionMet = GameManager.instance.CheckCondition(conditionToCheck);

        if (conditionMet) {
            RuntimeManager.PlayOneShot(eventName, transform.position);
            if (isPuzzlePort) {
                GameManager.instance.SwitchToPuzzle(id, level, !alreadyInserted);
            } else {
                if (alreadyInserted) {
                    GameManager.instance.SwitchToMap(level, !alreadyInserted);
                } else {
                    StartCoroutine(Hacking());
                }
            }
            alreadyInserted = true;
        }
    }

    IEnumerator Hacking()
    {
        GameObject hackingCanvas = GameObject.Find("HackingCanvas");
        hackingCanvas.GetComponent<HackingSlider>().ToggleSlider(true);

        Slider slider = hackingCanvas.GetComponentInChildren<Slider>();

        TooltipManager tooltipManager = GameObject.Find("TooltipCanvas").GetComponent<TooltipManager>();

        tooltipManager.ToggleClickTooltip(false);
        GameManager.instance.ToggleDialogueFreeze(true);

        float elapsedTime = 0;
        while (elapsedTime <= 2)
        {
            elapsedTime += Time.deltaTime;
            slider.value = elapsedTime / 2;
            yield return null;
        }
        
        hackingCanvas.GetComponent<HackingSlider>().ToggleSlider(false);
        GameManager.instance.ToggleDialogueFreeze(false);
        GameManager.instance.UpdateActivatedPanel(id, level);
        GameManager.instance.SwitchToMap(level, !alreadyInserted);
    }
}
