using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public bool wasVisited = false;

    public void TriggerDialogue()
    {
        if (wasVisited)
        {
            Debug.Log("Dialogue already visited!");
            return;
        }
        
        if (GameObject.Find("DialogueCanvas") != null && GameObject.Find("DialogueCanvas").activeSelf)
        {
            Debug.Log("Dialogue already active!");
            return;
        }

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        wasVisited = true;
    }
}
