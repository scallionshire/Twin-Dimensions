using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ExtrudableDataScriptable")]
public class ExtrudableDataScriptable : ScriptableObject
{
    public GameObject extrudable2DPrefab; // 2D extrudable prefab to instantiate

    public List<ExtrudableList> extrudableDataList;
}
