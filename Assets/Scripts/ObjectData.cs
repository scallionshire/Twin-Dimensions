using UnityEngine;

[System.Serializable]
public struct ObjectData
{
    public string name;
    public Vector3 position;
    public Sprite sprite;
    public int sortingOrder;
    public Color color;
    public RuntimeAnimatorController animatorController;
    public ColliderData colliderData;
    public DialogueData dialogueData;
}