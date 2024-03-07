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
        if (GameObject.Find("DialogueCanvas") != null && GameObject.Find("DialogueCanvas").activeSelf)
        {
            Debug.Log("Dialogue already active!");
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
}
