using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlaySFX : MonoBehaviour
{
    public EventReference SFXEvent;

    public void Play() {
        RuntimeManager.PlayOneShot(SFXEvent, transform.position);
    }
}
