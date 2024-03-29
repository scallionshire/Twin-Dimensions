using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineActivator : MonoBehaviour
{
    private PlayableDirector director;
    private DialogueManager dialogueManager;
    private bool timelinePlayed = false;
    private bool timelineComplete = false;

    public bool checkUSBInserted = false;
    public bool doNotUnfreeze = false;

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

            if (timelineComplete && !dialogueManager.dialogueActive)
            {
                StopTimeline();
            }
        }
    }

    public void PlayTimeline()
    {   
        // Handle the PC case
        if (checkUSBInserted && !GameManager.instance.gameState.USBInserted)
        {
            return;
        }

        if (!timelinePlayed)
        {
            // Freeze first so that the player can't fuck w the camera while the camera transition is occurring
            GameManager.instance.ToggleDialogueFreeze(true);
            timelineComplete = false;
            director.Play();
            timelinePlayed = true;

            // Get a reference to the dialogue manager so we can check if its active later
            dialogueManager = GameObject.Find("GameManager").GetComponent<DialogueManager>();
        }
    }

    public void StopTimeline()
    {
        director.Stop();

        if (!doNotUnfreeze) {
            // Don't allow player to control camera too early while it's still transitioning back
            GameObject mainCamera = GameObject.Find("Main Camera");
            StartCoroutine(WaitBeforeUnfreezing(mainCamera));
        }
    }

    IEnumerator WaitBeforeUnfreezing(GameObject mainCamera) {
        CinemachineBrain cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();

        // Wait for blending to start
        yield return new WaitUntil(() => cinemachineBrain.IsBlending);
        // Wait for blending to stop
        yield return new WaitUntil(() => !cinemachineBrain.IsBlending);
        
        GameManager.instance.ToggleDialogueFreeze(false);
    }
}
