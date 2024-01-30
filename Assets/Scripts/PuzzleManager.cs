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
        public bool isCorrect;
    }

    public List<PuzzlePiece> correctBlocks;
    public Sprite correctSprite;
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
            StartCoroutine(changeScene());
        }
    }

    IEnumerator changeScene()
    {
        if (gameManager != null && audioManager != null) {
            audioManager.Play("USB");
            gameManager.doorUnlocked = true;
        }
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("tutroom");
    }

    bool IsPuzzleSolved()
    {
        bool isSolved = true;
        // Iterate through each cell in the tilemap
        foreach (PuzzlePiece piece in correctBlocks)
        {
            if (!piece.isCorrect)
            {
                // Check for the right tile here
                isSolved = false;
            } else 
            {
                Debug.Log("Correct piece found: " + piece.destinationObject.name);
                piece.destinationObject.GetComponent<SpriteRenderer>().sprite = correctSprite;
            }
        }

        return isSolved;
    }
}