using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FootstepSFX : MonoBehaviour
{
    public EventReference walkEvent;
    public EventReference runEvent;
    public EventReference jumpEvent;
    public EventReference landEvent;

    public void WalkStep() {
        RuntimeManager.PlayOneShot(walkEvent, transform.position);
    }

    public void RunStep() {
        RuntimeManager.PlayOneShot(runEvent, transform.position);
    }

    public void Jump() {
        RuntimeManager.PlayOneShot(jumpEvent, transform.position);
    }

    public void Land() {
        RuntimeManager.PlayOneShot(landEvent, transform.position);
    }
}
