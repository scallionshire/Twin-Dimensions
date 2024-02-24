using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PuzzlePiece {
    public GameObject destinationObject; // reference to destination object
    public Sprite correctSprite;
    public bool isCorrect;
}

public class PuzzleManager : MonoBehaviour
{
    public List<PuzzlePiece> correctBlocks;
    private GameManager gameManager;
    public int currentPuzzleId;
    public List<bool> puzzleSolved;
    public BlockPuzzles levelPuzzles;

    void Start()
    {
        if (GameObject.Find("GameManager") != null) {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        puzzleSolved = new List<bool>(levelPuzzles.puzzles[0].puzzleBlocks.Count);

        for (int i = 0; i < levelPuzzles.puzzles[0].puzzleBlocks.Count; i++) {
            Debug.Log(i);
            GameObject newDestination = Instantiate(levelPuzzles.destinationPrefab, levelPuzzles.puzzles[0].puzzleBlocks[i].destinationPosition, Quaternion.identity);

            newDestination.GetComponent<SpriteRenderer>().sprite = levelPuzzles.puzzles[0].puzzleBlocks[i].destinationSprite;
            newDestination.name = levelPuzzles.puzzles[0].puzzleBlocks[i].destinationName;
            newDestination.GetComponent<BlockScript>().blockId = i;
            newDestination.GetComponent<BlockScript>().blockName = levelPuzzles.puzzles[0].puzzleBlocks[i].blockName;

            GameObject newBlock = Instantiate(levelPuzzles.blockPrefab, levelPuzzles.puzzles[0].puzzleBlocks[i].blockInitPosition, Quaternion.identity);

            newBlock.GetComponent<SpriteRenderer>().sprite = levelPuzzles.puzzles[0].puzzleBlocks[i].blockSprite;
            newBlock.name = levelPuzzles.puzzles[0].puzzleBlocks[i].blockName;

            PuzzlePiece piece = new PuzzlePiece() {
                destinationObject = newDestination,
                correctSprite = levelPuzzles.puzzles[0].puzzleBlocks[i].correctSprite,
                isCorrect = false
            };

            correctBlocks.Add(piece);
        }
    }

    void Update()
    {
        // Check for puzzle completion
        if (IsPuzzleSolved())
        {
            // Puzzle is solved, provide feedback and handle progression
            Debug.Log("Puzzle solved");
            gameManager.gameState.DoorUnlocked = true;
        }
    }

    bool IsPuzzleSolved()
    {
        bool isSolved = true;

        int index = 0;
        // Iterate through each cell in the tilemap
        foreach (PuzzlePiece piece in correctBlocks)
        {
            if (!piece.isCorrect)
            {
                isSolved = false;
            } else 
            {
                Debug.Log("Correct piece found");
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("BlockTrigger")) {
                    if (go.GetComponent<BlockScript>().blockId == index) {
                        go.GetComponent<SpriteRenderer>().sprite = piece.correctSprite;
                        go.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        break;
                    }
                }

                // TODO: change this from being hardcoded later
                switch (piece.destinationObject.name) {
                    case "elevatorTrigger":
                        gameManager.gameState.ElevatorBlockSet = true;
                        gameManager.gameState.completedBlocks.Add(0);
                        break;
                    case "pinkTrigger":
                        gameManager.gameState.PinkBlockSet = true;
                        gameManager.gameState.completedBlocks.Add(2);
                        break;
                    case "greenTrigger":
                        gameManager.gameState.GreenBlockSet = true;
                        gameManager.gameState.completedBlocks.Add(1);
                        break;
                    case "yellowTrigger":
                        gameManager.gameState.YellowBlockSet = true;
                        gameManager.gameState.completedBlocks.Add(3);
                        break;
                }
            }
        }

        return isSolved;
    }
}