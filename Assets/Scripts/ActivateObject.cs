using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObject : MonoBehaviour
{
    public GameObject objectToActivate;
    public GameStateCondition conditionToCheck;
    public bool doNotRepeat;
    private bool activated = false;

    public void Activate()
    {
        bool conditionMet = GameManager.instance.CheckCondition(conditionToCheck);

        if (conditionMet && !activated)
        {
            objectToActivate.SetActive(true);
            if (doNotRepeat)
            {
                activated = true;
            }
        }
    }

    public void Deactivate()
    {
        objectToActivate.SetActive(false);
    }
}
