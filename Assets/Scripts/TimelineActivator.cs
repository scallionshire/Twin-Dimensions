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

    public GameStateCondition conditionToCheck = GameStateCondition.insertedUSB;
    public bool checkCondition = false;
    private bool conditionMet = false;

    [Space(10)]
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
            case GameStateCondition.noUSB:
                conditionMet = !GameManager.instance.gameState.PlayerHasUSB;
                break;
        }

        // Handle the PC case
        if (checkCondition && !conditionMet)
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
