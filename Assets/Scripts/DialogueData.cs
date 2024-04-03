using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DialogueData {
    public bool doNotRepeat;
    public bool isCutscene;
    public DialogueCondition conditionToCheck;
    public Dialogue preConditionDialogue;
    public Dialogue postConditionDialogue;
}