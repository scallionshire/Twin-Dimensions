using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string dialogueName;
    public Sentence[] sentences;
}

[System.Serializable]
public class DialogueObject
{
    public Vector3 position;
    public Vector3 scale;
    public Dialogue dialogue;
}