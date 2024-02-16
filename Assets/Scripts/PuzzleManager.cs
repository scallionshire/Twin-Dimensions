using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleManager : MonoBehaviour
{
    [System.Serializable]
    public struct PuzzlePiece {
        public GameObject destinationObject;
        public Sprite correctSprite;
        public bool isCorrect;
    }

    public List<PuzzlePiece> correctBlocks;
    private AudioManager audioManager;
    private GameManager gameManager;

    void Start()
    {
        if (GameObject.Find("GameManager") != null) {
            audioManager = GameObject.Find("GameManager").GetComponent<AudioManager>();
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        // Iterate through each cell in the tilemap
        foreach (PuzzlePiece piece in correctBlocks)
        {
            if (!piece.isCorrect)
            {
                isSolved = false;
            } else 
            {
                Debug.Log("Correct piece found: " + piece.destinationObject.name);
                piece.destinationObject.GetComponent<SpriteRenderer>().sprite = piece.correctSprite;
                piece.destinationObject.GetComponent<SpriteRenderer>().sortingOrder = 3;

                if (piece.destinationObject.name == "elevatorTrigger") {
                    gameManager.gameState.ElevatorBlockSet = true;
                    gameManager.gameState.completedBlocks.Add(0);
                } else if (piece.destinationObject.name == "pinkTrigger") {
                    gameManager.gameState.PinkBlockSet = true;
                    gameManager.gameState.completedBlocks.Add(2);
                } else if (piece.destinationObject.name == "greenTrigger") {
                    gameManager.gameState.GreenBlockSet = true;
                    gameManager.gameState.completedBlocks.Add(1);
                } else if (piece.destinationObject.name == "yellowTrigger") {
                    gameManager.gameState.YellowBlockSet = true;
                    gameManager.gameState.completedBlocks.Add(3);
                }
            }
        }

        return isSolved;
    }
}