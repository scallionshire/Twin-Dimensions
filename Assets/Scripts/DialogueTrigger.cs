using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DialogueTrigger : MonoBehaviour
{
    public bool isCutsceneDialogue = false;
    private bool cutsceneWasVisited = false;

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

        if ((isCutsceneDialogue && !cutsceneWasVisited) || !isCutsceneDialogue)
        {
            if (GameManager.instance.gameState.USBInserted)
            {
                gm.GetComponent<DialogueManager>().StartDialogue(withUSBDialogue, isCutsceneDialogue);

                // We don't want to play cutscene dialogues more than once
                if (isCutsceneDialogue)
                {
                    cutsceneWasVisited = true;
                }
            }
            else
            {
                gm.GetComponent<DialogueManager>().StartDialogue(noUSBDialogue, isCutsceneDialogue);
                if (isCutsceneDialogue)
                {
                    cutsceneWasVisited = true;
                }
            }
        }
    }
}
