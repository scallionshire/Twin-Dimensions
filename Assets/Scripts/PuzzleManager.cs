using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public bool[] puzzlesSolved;
    public PuzzleDataScriptable levelPuzzles;

    public void LoadPuzzle()
    {   
        correctBlocks.Clear();

        if (GameObject.Find("GameManager") != null) {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            currentPuzzleId = gameManager.gameState.CurrentPuzzleId;
        } else {
            currentPuzzleId = -1; // TODO: this should be -1 in the real game
        }

        puzzlesSolved = new bool[levelPuzzles.puzzles.Count];

        if (currentPuzzleId == -1) { // map should be blank, no puzzles loaded in
            Debug.Log("No puzzle selected");
            return;
        }

        GameObject background = GameObject.Find("Background");
        background.GetComponent<SpriteRenderer>().sprite = levelPuzzles.backgroundSprite;
        background.transform.localScale = levelPuzzles.backgroundScale;
        background.transform.position = levelPuzzles.backgroundPosition;
        background.GetComponent<SpriteRenderer>().color = levelPuzzles.backgroundColor;

        GameObject rec = GameObject.Find("Rec");
        rec.transform.localScale = levelPuzzles.recScale;
        rec.transform.position = levelPuzzles.recPosition;

        for (int i = 0; i < levelPuzzles.wallPositions.Count; i++) {
            GameObject wall = GameObject.Find("Wall" + i);
            wall.transform.position = levelPuzzles.wallPositions[i];
        }

        // Load in puzzle
        for (int i = 0; i < levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks.Count; i++) {
            // Instantiate destination blocks
            GameObject newDestination = Instantiate(levelPuzzles.destinationPrefab, levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[i].destinationPosition, Quaternion.identity);
            newDestination.AddComponent<BoxCollider2D>();

            PuzzlePiece piece = new PuzzlePiece() {
                destinationObject = newDestination,
                correctSprite = levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[i].correctSprite,
                isCorrect = false
            };

            if (levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[i].isSolved) {
                newDestination.GetComponent<SpriteRenderer>().sprite = levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[i].correctSprite;
                piece.isCorrect = true;

                newDestination.name = levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[i].destinationName;
                newDestination.GetComponent<BlockScript>().blockId = i;
                newDestination.GetComponent<BlockScript>().blockName = levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[i].blockName;
            } else {
                newDestination.GetComponent<SpriteRenderer>().sprite = levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[i].destinationSprite;

                newDestination.name = levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[i].destinationName;
                newDestination.GetComponent<BlockScript>().blockId = i;
                newDestination.GetComponent<BlockScript>().blockName = levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[i].blockName;

                // Instantiate movable puzzle blocks
                GameObject newBlock = Instantiate(levelPuzzles.blockPrefab, levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[i].blockInitPosition, Quaternion.identity);

                newBlock.GetComponent<SpriteRenderer>().sprite = levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[i].blockSprite;
                newBlock.name = levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[i].blockName;
            }

            correctBlocks.Add(piece);
        }

        for (int k = 0; k < levelPuzzles.puzzles[currentPuzzleId].circuitSprites.Count; k++) {
            // Instantiate environment sprites
            GameObject newWire = Instantiate(levelPuzzles.circuitPrefab, levelPuzzles.puzzles[currentPuzzleId].circuitSprites[k].circuitInitPosition, Quaternion.identity);

            if ((k < levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks.Count && levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[k].isSolved)|| levelPuzzles.puzzles[currentPuzzleId].circuitSprites[k].circuitName.Contains("env")) {
                newWire.GetComponent<SpriteRenderer>().sprite = levelPuzzles.puzzles[currentPuzzleId].circuitSprites[k].circuitSprite;
                newWire.name = levelPuzzles.puzzles[currentPuzzleId].circuitSprites[k].circuitName;
            } else {
                newWire.GetComponent<SpriteRenderer>().sprite = levelPuzzles.puzzles[currentPuzzleId].circuitSprites[k].circuitSprite;
                newWire.name = levelPuzzles.puzzles[currentPuzzleId].circuitSprites[k].circuitName;

                Color tmp = newWire.GetComponent<SpriteRenderer>().color;
                tmp.a = 0.3f;
                newWire.GetComponent<SpriteRenderer>().color = tmp;
            }
        }
    }

    void Update()
    {
        // Check for puzzle completion
        if (currentPuzzleId >= 0 && IsPuzzleSolved())
        {
            // Puzzle is solved, provide feedback and handle progression
            puzzlesSolved[currentPuzzleId] = true;
            gameManager?.SolvePuzzle(levelPuzzles.level, currentPuzzleId);
            
            if (GameObject.Find("TooltipCanvas") != null)
            {
                GameObject.Find("TooltipCanvas").GetComponent<TooltipManager>().ShowQTooltip();
            }
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
                string blockName = levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[index].blockName;

                // Update game state
                gameManager?.SolvePuzzleBlock(currentPuzzleId, index, levelPuzzles.level);

                // Visual indicator for success state goes here
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("BlockTrigger")) {
                    if (go.GetComponent<BlockScript>().blockName == levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[index].blockName) {
                        // Replace trigger block with the correct sprite
                        piece.destinationObject.GetComponent<SpriteRenderer>().sprite = piece.correctSprite;

                        break;
                    }
                }

                if (levelPuzzles.circuitPrefab != null) {
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("Connector")) {
                        if (go.name == levelPuzzles.puzzles[currentPuzzleId].circuitSprites[index].circuitName) {
                            // Make corresponding connector opaque
                            Color tmp = go.GetComponent<SpriteRenderer>().color;
                            tmp.a = 1f;
                            go.GetComponent<SpriteRenderer>().color = tmp;

                            break;
                        }
                    }
                }

                Destroy(GameObject.Find(blockName));
            }

            index++;
        }
        return isSolved;
    }
}