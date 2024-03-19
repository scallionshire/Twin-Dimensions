using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue noUSBDialogue;
    public Dialogue withUSBDialogue;
    private bool noUSBWasVisited = false;
    private bool withUSBWasVisited = false;

    public void TriggerDialogue()
    {
        GameObject dc = GameObject.Find("DialogueCanvas");

        if (dc == null) {
            Debug.Log("harro????");
        }
        
        GameObject gm = GameObject.Find("GameManager");

        if (dc != null && gm.GetComponent<DialogueManager>().dialogueActive)
        {
            return;
        }

        if (FindObjectOfType<GameManager>().gameState.USBInserted && !withUSBWasVisited)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(withUSBDialogue);
            withUSBWasVisited = true;
        }
        else if (!noUSBWasVisited)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(noUSBDialogue);
            noUSBWasVisited = true;
        }
    }

    public void MakeVisited()
    {
        noUSBWasVisited = true;
        withUSBWasVisited = true;
    }
}
