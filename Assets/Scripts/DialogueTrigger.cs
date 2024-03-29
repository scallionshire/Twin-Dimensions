using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DialogueTrigger : MonoBehaviour
{
    public bool doNotRepeat = false;
    public bool isCutscene = false;

    private bool noUSBVisited = false;
    private bool withUSBVisited = false;

    public Dialogue noUSBDialogue;
    public Dialogue withUSBDialogue;

    public void TriggerDialogue()
    {
        GameObject dc = GameObject.Find("DialogueCanvas");
        GameObject gm = GameObject.Find("GameManager");

        if (dc != null && gm.GetComponent<DialogueManager>().dialogueActive)
        {
            return;
        }

        if (GameManager.instance.gameState.USBInserted && ((doNotRepeat && !withUSBVisited) || !doNotRepeat))
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
        else if (!GameManager.instance.gameState.USBInserted && ((doNotRepeat && !noUSBVisited) || !doNotRepeat))
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
