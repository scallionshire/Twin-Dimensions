using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DialogueTrigger : MonoBehaviour
{
    public bool doNotRepeat = false;
    public bool isCutscene = false;

    [Space(10)]
    public GameStateCondition conditionToCheck = GameStateCondition.insertedUSB;
    private bool conditionMet = false;

    private bool noUSBVisited = false;
    private bool withUSBVisited = false;

    [Space(10)]
    public Dialogue noUSBDialogue;
    public Dialogue withUSBDialogue;

    public void TriggerDialogue()
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
            case GameStateCondition.door0Open:
                conditionMet = GameManager.instance.gameState.Door0Unlocked;
                break;
        }

        GameObject dc = GameObject.Find("DialogueCanvas");
        GameObject gm = GameObject.Find("GameManager");

        if (dc != null && gm.GetComponent<DialogueManager>().dialogueActive)
        {
            return;
        }

        if (conditionMet && ((doNotRepeat && !withUSBVisited) || !doNotRepeat))
        {
            if (GameManager.instance.ActiveSceneName == "new3Dtut")
            {
                gm.GetComponent<DialogueManager>().StartDialogue(withUSBDialogue, isCutscene);
            } else {
                // We don't have camera transition cutscenes in the 2d scenes, so leave them be
                gm.GetComponent<DialogueManager>().StartDialogue(withUSBDialogue, false);
            }

            // We don't want to play cutscene dialogues more than once
            if (doNotRepeat)
            {
                withUSBVisited = true;
            }
        }
        else if (!conditionMet && ((doNotRepeat && !noUSBVisited) || !doNotRepeat))
        {
            if (GameManager.instance.ActiveSceneName == "new3Dtut")
            {
                gm.GetComponent<DialogueManager>().StartDialogue(noUSBDialogue, isCutscene);
            } else {
                // We don't want the fancy stuff for the 2D scenes
                gm.GetComponent<DialogueManager>().StartDialogue(noUSBDialogue, false);
            }
            
            if (doNotRepeat)
            {
                noUSBVisited = true;
            }
        }
    }
}
