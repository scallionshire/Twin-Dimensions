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