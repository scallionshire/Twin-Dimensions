using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[System.Serializable]
public struct PuzzlePiece {
    public GameObject destinationObject; // reference to destination object
    public Sprite correctSprite;
    public bool isCorrect;
}

public class PuzzleManager : MonoBehaviour
{
    [TextArea]
    [Tooltip("Doesn't do anything. Just comments shown in inspector")]
    public string Note = "In order for TopDownManager to work, the scene MUST HAVE parent gameobjects called 'Objects', 'Doors', 'USBPorts', 'Walls', and 'Extrudables'.";

    [Header("DEBUG ONLY")]
    public bool debugMode = false;
    public PuzzleDataScriptable levelPuzzles;
    public int currentPuzzleId;

    [Header("Puzzle Data")]
    public List<PuzzlePiece> correctBlocks;
    public bool[] puzzlesSolved;
    private bool firstTimeFinished = false;

    // ----------- SCENE LOAD FUNCTIONS -----------
    void Start()
    {
        if (debugMode) {
            LoadPuzzle();
        }
    }

    public void LoadPuzzle()
    {   
        firstTimeFinished = false;

        correctBlocks.Clear();

        // Set player to center of screen
        GameObject player = GameObject.Find("2D Player");
        player.transform.position = GameObject.Find("Background").transform.position;

        if (!debugMode) {
            currentPuzzleId = GameManager.instance.gameState.CurrentPuzzleId;
            switch (GameManager.instance.gameState.CurrentLevel) {
                case Level.tutorial:
                    levelPuzzles = GameManager.instance.tutorialPuzzle;
                    break;
                case Level.computerlab:
                    levelPuzzles = GameManager.instance.computerPuzzle;
                    break;
            }
        }

        GameObject background = GameObject.Find("Background");
        background.GetComponent<SpriteRenderer>().sprite = levelPuzzles.backgroundSprite;
        background.transform.localScale = levelPuzzles.backgroundScale;
        background.transform.position = levelPuzzles.backgroundPosition;
        background.GetComponent<SpriteRenderer>().color = levelPuzzles.backgroundColor;

        GameObject frame = GameObject.Find("Frame");
        frame.GetComponent<SpriteRenderer>().size = levelPuzzles.frameSize;
        frame.transform.position = levelPuzzles.framePosition;

        for (int i = 0; i < levelPuzzles.wallPositions.Count; i++) {
            GameObject wall = GameObject.Find("Wall" + i);
            wall.transform.localPosition = levelPuzzles.wallPositions[i];
        }

        if (currentPuzzleId == -1) { // map should be blank, no puzzles loaded in
            Debug.Log("No puzzle selected");
            return;
        }

        for (int i = 0; i < levelPuzzles.puzzles[currentPuzzleId].dialogues.Count; i++) {
            GameObject newDialogueTrigger = Instantiate(levelPuzzles.dialogueTriggerPrefab, levelPuzzles.puzzles[currentPuzzleId].dialogues[i].position, Quaternion.identity);
            newDialogueTrigger.transform.localScale = levelPuzzles.puzzles[currentPuzzleId].dialogues[i].scale;
            newDialogueTrigger.GetComponent<DialogueTrigger>().withUSBDialogue = levelPuzzles.puzzles[currentPuzzleId].dialogues[i].dialogue;
            newDialogueTrigger.GetComponent<DialogueTrigger>().name = levelPuzzles.puzzles[currentPuzzleId].dialogues[i].dialogue.dialogueName;
        }

        puzzlesSolved = new bool[levelPuzzles.puzzles.Count];

        // Load in puzzle
        for (int i = 0; i < levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks.Count; i++) {
            // Instantiate destination blocks
            GameObject newDestination = Instantiate(levelPuzzles.destinationPrefab, levelPuzzles.puzzles[currentPuzzleId].puzzleBlocks[i].destinationPosition, Quaternion.identity);

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
                tmp.a = 0.6f;
                newWire.GetComponent<SpriteRenderer>().color = tmp;
            }
        }
    }

    public void SavePuzzle()
    {
        PuzzleList puzzle = new PuzzleList();

        puzzle.puzzleBlocks = new List<PuzzleSet>();
        puzzle.circuitSprites = new List<CircuitSet>();
        puzzle.dialogues = new List<DialogueObject>();

        // Save puzzle block data
        foreach (GameObject blockSlot in GameObject.FindGameObjectsWithTag("BlockTrigger")) {
            PuzzleSet blockData = new PuzzleSet();

            GameObject block = GameObject.Find(blockSlot.GetComponent<BlockScript>().blockName);

            if (block == null) {
                Debug.Log("Matching block not found: " + blockSlot.GetComponent<BlockScript>().blockName);
                continue;
            }

            blockData.blockInitPosition = block.transform.position;
            blockData.blockSprite = block.GetComponent<SpriteRenderer>().sprite;
            blockData.blockName = block.name;

            blockData.destinationPosition = blockSlot.transform.position;
            blockData.destinationSprite = blockSlot.GetComponent<SpriteRenderer>().sprite;
            blockData.destinationName = blockSlot.name;

            puzzle.puzzleBlocks.Add(blockData);
        }

        // Save circuit data
        foreach (GameObject connector in GameObject.FindGameObjectsWithTag("Connector")) {
            CircuitSet circuitData = new CircuitSet();

            circuitData.circuitInitPosition = connector.transform.position;
            circuitData.circuitSprite = connector.GetComponent<SpriteRenderer>().sprite;
            circuitData.circuitName = connector.name;

            puzzle.circuitSprites.Add(circuitData);
        }

        // Save dialogue data
        foreach (GameObject dialogueTrigger in GameObject.FindGameObjectsWithTag("DialogueTrigger")) {
            DialogueObject dialogueData = new DialogueObject();

            dialogueData.position = dialogueTrigger.transform.position;
            dialogueData.scale = dialogueTrigger.transform.localScale;
            dialogueData.dialogue = dialogueTrigger.GetComponent<DialogueTrigger>().withUSBDialogue;

            puzzle.dialogues.Add(dialogueData);
        }

        while (levelPuzzles.puzzles.Count <= currentPuzzleId) {
            levelPuzzles.puzzles.Add(new PuzzleList());
        }

        levelPuzzles.puzzles[currentPuzzleId] = puzzle;
    }

    void Update()
    {
        // Check for puzzle completion
        if (currentPuzzleId >= 0 && IsPuzzleSolved())
        {
            // Puzzle is solved, provide feedback and handle progression
            puzzlesSolved[currentPuzzleId] = true;

            if (!firstTimeFinished) {
                StartCoroutine(PuzzleCompletionReturn());
            }
        }
    }

    IEnumerator PuzzleCompletionReturn()
    {
        firstTimeFinished = true;
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.SolvePuzzle(levelPuzzles.level, currentPuzzleId);
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

                if (levelPuzzles.level == Level.computerlab) {
                    Debug.Log("Solving puzzle block for index " + index);
                    GameManager.instance.SolvePuzzleBlock(levelPuzzles.level, currentPuzzleId, index);
                }

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