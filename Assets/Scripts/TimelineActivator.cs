using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineActivator : MonoBehaviour
{
    private PlayableDirector director;
    private DialogueManager dialogueManager;
    private bool timelinePlayed = false;
    private bool timelineComplete = false;

    public bool checkUSBInserted = false;

    // Start is called before the first frame update
    void Start()
    {
        director = GetComponent<PlayableDirector>();
    }

    void Update()
    {
        if (director.state == PlayState.Playing)
        {
            if (director.duration - director.time <= 0.1f)
            {
                timelineComplete = true;
            }
        }

        if (timelineComplete && !dialogueManager.dialogueActive)
        {
            StopTimeline();
        }
    }

    public void PlayTimeline()
    {
        if (checkUSBInserted && !GameManager.instance.gameState.USBInserted)
        {
            return;
        }

        if (!timelinePlayed)
        {
            timelineComplete = false;
            director.Play();
            timelinePlayed = true;
            dialogueManager = GameObject.Find("GameManager").GetComponent<DialogueManager>();

            GameManager.instance.ToggleCutsceneFreeze(true);
        }
    }

    public void StopTimeline()
    {
        director.Stop();

        if (dialogueManager.currentDialogueName != "ComputerFirstPlug") {
            Debug.Log("Toggling Cutscene Freeze");
            GameManager.instance.ToggleCutsceneFreeze(false);
        }
    }
}
