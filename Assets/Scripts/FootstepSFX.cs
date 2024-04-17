using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FootstepSFX : MonoBehaviour
{
    public EventReference footstepEvent;

    public void Step() {
        RuntimeManager.PlayOneShot(footstepEvent, transform.position);
    }
}
