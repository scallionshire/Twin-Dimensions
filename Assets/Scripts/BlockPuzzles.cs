using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockPuzzles
{
    // Level that this 2d block puzzle belongs to
    public Level level;
    public GameObject destinationPrefab; // destination prefab to instantiate
    public GameObject blockPrefab; // block prefab to instantiate
    public GameObject circuitPrefab; // circuit environment prefab to instantiate
    public List<PuzzleList> puzzles;
}