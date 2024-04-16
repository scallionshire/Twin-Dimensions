using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct USBPortData
{
    public string name;
    public int portID;
    public Vector3 position;
    public Sprite sprite;
    public Level level;
    public ColliderData colliderData;
    public DialogueData dialogueData;
}