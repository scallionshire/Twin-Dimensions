using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PuzzleSet {
    public Vector3 blockInitPosition;
    public Sprite blockSprite;
    public string blockName;
    public Sprite correctSprite;
    
    public Vector3 destinationPosition;
    public Sprite destinationSprite;
    public string destinationName;

    public bool isSolved;
}

[System.Serializable]
public struct CircuitSet {
    public Vector3 circuitInitPosition;
    public Sprite circuitSprite;
    public string circuitName;
}

[System.Serializable]
public class PuzzleList {
    public List<CircuitSet> circuitSprites;
    public List<PuzzleSet> puzzleBlocks;
}

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/BlockPuzzleScriptableObject")]
public class BlockPuzzles : ScriptableObject
{
    // Level that this 2d block puzzle belongs to
    public Level level;
    public GameObject destinationPrefab; // destination prefab to instantiate
    public GameObject blockPrefab; // block prefab to instantiate
    public GameObject circuitPrefab; // circuit environment prefab to instantiate
    public List<PuzzleList> puzzles;
}