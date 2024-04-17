using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ExtrudableData {
    public int id;
    public TransformData transformData;
    public Vector3 direction;
    public float amount;
    public bool shouldExtrude;
    public bool shouldLoop;
    public bool alreadyExtruded;
}

[System.Serializable]
public class ExtrudableList {
    public List<ExtrudableData> extrudableSets;
    public List<DialogueObject> dialogues;
}