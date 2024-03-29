using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DialogueTrigger : MonoBehaviour
{
    public bool doNotRepeat = false;
    public bool isCutscene = false;

    [Space(10)]
    public DialogueCondition conditionToCheck = DialogueCondition.insertedUSB;
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
            case DialogueCondition.hasUSB:
                conditionMet = GameManager.instance.gameState.PlayerHasUSB;
                break;
            case DialogueCondition.insertedUSB:
                conditionMet = GameManager.instance.gameState.USBInserted;
                break;
            case DialogueCondition.hasBattery:
                conditionMet = GameManager.instance.gameState.BatteriesCollected > 0;
                break;
        }

        Debug.Log("Has condition been met? " + conditionMet);

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
