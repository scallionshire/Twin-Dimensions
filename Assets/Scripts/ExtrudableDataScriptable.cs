using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ExtrudableDataScriptable")]
public class ExtrudableDataScriptable : ScriptableObject
{
    public Level level;
    
    public GameObject extrudable2DPrefab; // 2D extrudable prefab to instantiate

    public Sprite mapSprite; // map sprite to display
    public Vector3 mapPosition; // map position
    public Vector3 mapScale; // map scale

    public Vector3 recPosition; // rec overlay position
    public Vector3 recScale; // rec overlay scale
    
    public List<Vector3> wallPositions; // wall positions

    public List<ExtrudableList> extrudableDataList;
}
