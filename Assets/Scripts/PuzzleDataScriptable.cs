using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PuzzleDataScriptable")]
public class PuzzleDataScriptable : ScriptableObject
{
    // Level that this 2d block puzzle belongs to
    public Level level;

    public GameObject destinationPrefab; // destination prefab to instantiate
    public GameObject blockPrefab; // block prefab to instantiate
    public GameObject circuitPrefab; // circuit environment prefab to instantiate

    public Sprite backgroundSprite;
    public Vector3 backgroundScale;
    public Vector3 backgroundPosition;
    public Color backgroundColor;
    public Vector3 recScale;
    public Vector3 recPosition;

    public List<Vector3> wallPositions;

    public List<PuzzleList> puzzles;
}