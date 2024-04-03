using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WallData
{
    public TransformData transformData;
    public ColliderData colliderData;
    internal Vector3 position;
}