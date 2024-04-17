using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ColliderData
{
    public bool noCollider;
    public bool isTrigger;
    public Vector2 offset;
    public Vector2 size;
}