using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlaySFX : MonoBehaviour
{
    public EventReference SFXEvent;
    public GameStateCondition conditionToCheck;
    public bool doNotRepeat;
    private bool played = false;

    public void Play() {
        bool conditionMet = GameManager.instance.CheckCondition(conditionToCheck);

        if (conditionMet && !played)
        {
            RuntimeManager.PlayOneShot(SFXEvent, transform.position);
            if (doNotRepeat)
            {
                played = true;
            }
        }
    }
}
