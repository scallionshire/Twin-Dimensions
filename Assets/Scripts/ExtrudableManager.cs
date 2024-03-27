using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrudableManager : MonoBehaviour
{
    public int currentExtrudableSetId;
    public ExtrudableDataScriptable extrudableData;

    private GameManager gameManager;

    private List<GameObject> currentExtrudables = new List<GameObject>();

    public List<PuzzlePiece> correctBlocks;
    public bool[] puzzlesSolved;

    public void Start()
    {
        // currentExtrudables.Clear();

        // if (GameObject.Find("GameManager") != null) {
        //     gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //     currentExtrudableSetId = gameManager.gameState.CurrentExtrudableSetId;
        //     switch (gameManager.gameState.CurrentLevel) {
        //         case Level.tutorial:
        //             extrudableData = gameManager.initTutorialExtrudables;
        //             break;
        //         case Level.computerlab:
        //             extrudableData = gameManager.initComputerLabExtrudables;
        //             break;
        //     }
        // } else {
        //     currentExtrudableSetId = -1;
        // }

        GameObject background = GameObject.Find("Background");
        background.GetComponent<SpriteRenderer>().sprite = extrudableData.mapSprite;
        background.transform.localScale = extrudableData.mapScale;
        background.transform.position = extrudableData.mapPosition;

        GameObject rec = GameObject.Find("Rec");
        rec.transform.localScale = extrudableData.recScale;
        rec.transform.position = extrudableData.recPosition;

        for (int i = 0; i < extrudableData.wallPositions.Count; i++) {
            GameObject wall = GameObject.Find("Wall" + i);
            wall.transform.localPosition = extrudableData.wallPositions[i];
        }

        if (currentExtrudableSetId == -1) { // map should be blank, no puzzles loaded in
            Debug.Log("No extrudables selected");
            return;
        }

        // for (int i = 0; i < extrudableData.extrudableDataList[currentExtrudableSetId].dialogues.Count; i++) {
        //     GameObject newDialogueTrigger = Instantiate(extrudableData.dialogueTriggerPrefab, extrudableData.extrudableDataList[currentExtrudableSetId].dialogues[i].position, Quaternion.identity);
        //     newDialogueTrigger.transform.localScale = extrudableData.extrudableDataList[currentExtrudableSetId].dialogues[i].scale;

        //     newDialogueTrigger.GetComponent<DialogueTrigger>().withUSBDialogue = extrudableData.extrudableDataList[currentExtrudableSetId].dialogues[i].dialogue;
        // }

        List<ExtrudableData> extrudableSets = extrudableData.extrudableDataList[currentExtrudableSetId].extrudableSets;
        for (int i = 0; i < extrudableSets.Count; i++) {
            GameObject newExtrudable = Instantiate(extrudableData.extrudable2DPrefab, extrudableSets[i].position, Quaternion.identity);
            newExtrudable.GetComponent<SpriteRenderer>().size = extrudableSets[i].size;
            newExtrudable.GetComponent<BoxCollider2D>().size = extrudableSets[i].size;
            newExtrudable.transform.rotation = Quaternion.Euler(extrudableSets[i].rotation);
            newExtrudable.name = "Extrudable" + extrudableSets[i].id;

            Extrudable newExtrudableScript = newExtrudable.GetComponent<Extrudable>();
            newExtrudableScript.extrudableId = extrudableSets[i].id;
            newExtrudableScript.extrudeDirection = extrudableSets[i].direction;
            newExtrudableScript.extrudeAmount = extrudableSets[i].amount;
            newExtrudableScript.shouldLoop = extrudableSets[i].shouldLoop;
            newExtrudableScript.isExtruding = extrudableSets[i].shouldExtrude;

            if (gameManager != null && gameManager.gameState.Extrudables[extrudableSets[i].id]) {
                newExtrudable.GetComponent<Extrudable>().MakeAlreadyExtruded();
            }

            currentExtrudables.Add(newExtrudable);
        }
        puzzlesSolved = new bool[extrudableData.extrudableDataList.Count];
        ExtrudableList currentExtrudablesSet=extrudableData.extrudableDataList[currentExtrudableSetId];
        // Load in puzzle
        for (int i = 0; i < currentExtrudablesSet.extrudableSets.Count; i++) {
            // Instantiate destination blocks
            GameObject newDestination = Instantiate(extrudableData.destinationPrefab, currentExtrudablesSet.puzzle.puzzleBlocks[i].destinationPosition, Quaternion.identity);

            PuzzlePiece piece = new PuzzlePiece() {
                destinationObject = newDestination,
                correctSprite = currentExtrudablesSet.puzzle.puzzleBlocks[i].correctSprite,
                isCorrect = false
            };

            if (currentExtrudablesSet.puzzle.puzzleBlocks[i].isSolved) {
                newDestination.GetComponent<SpriteRenderer>().sprite = currentExtrudablesSet.puzzle.puzzleBlocks[i].correctSprite;
                piece.isCorrect = true;

                newDestination.name = currentExtrudablesSet.puzzle.puzzleBlocks[i].destinationName;
                newDestination.GetComponent<BlockScript>().blockId = i;
                newDestination.GetComponent<BlockScript>().blockName = currentExtrudablesSet.puzzle.puzzleBlocks[i].blockName;
            } else {
                newDestination.GetComponent<SpriteRenderer>().sprite = currentExtrudablesSet.puzzle.puzzleBlocks[i].destinationSprite;

                newDestination.name = currentExtrudablesSet.puzzle.puzzleBlocks[i].destinationName;
                newDestination.GetComponent<BlockScript>().blockId = i;
                newDestination.GetComponent<BlockScript>().blockName = currentExtrudablesSet.puzzle.puzzleBlocks[i].blockName;

                // Instantiate movable puzzle blocks
                GameObject newBlock = Instantiate(extrudableData.blockPrefab, currentExtrudablesSet.puzzle.puzzleBlocks[i].blockInitPosition, Quaternion.identity);

                newBlock.GetComponent<SpriteRenderer>().sprite = currentExtrudablesSet.puzzle.puzzleBlocks[i].blockSprite;
                newBlock.name = currentExtrudablesSet.puzzle.puzzleBlocks[i].blockName;
            }

            correctBlocks.Add(piece);
        }

        for (int k = 0; k < currentExtrudablesSet.puzzle.circuitSprites.Count; k++) {
            // Instantiate environment sprites
            GameObject newWire = Instantiate(extrudableData.circuitPrefab, currentExtrudablesSet.puzzle.circuitSprites[k].circuitInitPosition, Quaternion.identity);

            if ((k < currentExtrudablesSet.puzzle.puzzleBlocks.Count && currentExtrudablesSet.puzzle.puzzleBlocks[k].isSolved)|| currentExtrudablesSet.puzzle.circuitSprites[k].circuitName.Contains("env")) {
                newWire.GetComponent<SpriteRenderer>().sprite = currentExtrudablesSet.puzzle.circuitSprites[k].circuitSprite;
                newWire.name = currentExtrudablesSet.puzzle.circuitSprites[k].circuitName;
            } else {
                newWire.GetComponent<SpriteRenderer>().sprite = currentExtrudablesSet.puzzle.circuitSprites[k].circuitSprite;
                newWire.name = currentExtrudablesSet.puzzle.circuitSprites[k].circuitName;

                Color tmp = newWire.GetComponent<SpriteRenderer>().color;
                tmp.a = 0.3f;
                newWire.GetComponent<SpriteRenderer>().color = tmp;
            }
        }
    }
    // Start is called before the first frame update
    public void LoadMap()
    {
        currentExtrudables.Clear();

        if (GameObject.Find("GameManager") != null) {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            currentExtrudableSetId = gameManager.gameState.CurrentExtrudableSetId;
            switch (gameManager.gameState.CurrentLevel) {
                case Level.tutorial:
                    extrudableData = gameManager.initTutorialExtrudables;
                    break;
                case Level.computerlab:
                    extrudableData = gameManager.initComputerLabExtrudables;
                    break;
            }
        } else {
            currentExtrudableSetId = -1;
        }

        GameObject background = GameObject.Find("Background");
        background.GetComponent<SpriteRenderer>().sprite = extrudableData.mapSprite;
        background.transform.localScale = extrudableData.mapScale;
        background.transform.position = extrudableData.mapPosition;

        GameObject rec = GameObject.Find("Rec");
        rec.transform.localScale = extrudableData.recScale;
        rec.transform.position = extrudableData.recPosition;

        for (int i = 0; i < extrudableData.wallPositions.Count; i++) {
            GameObject wall = GameObject.Find("Wall" + i);
            wall.transform.localPosition = extrudableData.wallPositions[i];
        }

        if (currentExtrudableSetId == -1) { // map should be blank, no puzzles loaded in
            Debug.Log("No extrudables selected");
            return;
        }

        for (int i = 0; i < extrudableData.extrudableDataList[currentExtrudableSetId].dialogues.Count; i++) {
            GameObject newDialogueTrigger = Instantiate(extrudableData.dialogueTriggerPrefab, extrudableData.extrudableDataList[currentExtrudableSetId].dialogues[i].position, Quaternion.identity);
            newDialogueTrigger.transform.localScale = extrudableData.extrudableDataList[currentExtrudableSetId].dialogues[i].scale;

            newDialogueTrigger.GetComponent<DialogueTrigger>().withUSBDialogue = extrudableData.extrudableDataList[currentExtrudableSetId].dialogues[i].dialogue;
        }

        List<ExtrudableData> extrudableSets = extrudableData.extrudableDataList[currentExtrudableSetId].extrudableSets;
        for (int i = 0; i < extrudableSets.Count; i++) {
            GameObject newExtrudable = Instantiate(extrudableData.extrudable2DPrefab, extrudableSets[i].position, Quaternion.identity);
            newExtrudable.GetComponent<SpriteRenderer>().size = extrudableSets[i].size;
            newExtrudable.GetComponent<BoxCollider2D>().size = extrudableSets[i].size;
            newExtrudable.transform.rotation = Quaternion.Euler(extrudableSets[i].rotation);
            newExtrudable.name = "Extrudable" + extrudableSets[i].id;

            Extrudable newExtrudableScript = newExtrudable.GetComponent<Extrudable>();
            newExtrudableScript.extrudableId = extrudableSets[i].id;
            newExtrudableScript.extrudeDirection = extrudableSets[i].direction;
            newExtrudableScript.extrudeAmount = extrudableSets[i].amount;
            newExtrudableScript.shouldLoop = extrudableSets[i].shouldLoop;
            newExtrudableScript.isExtruding = extrudableSets[i].shouldExtrude;

            if (gameManager != null && gameManager.gameState.Extrudables[extrudableSets[i].id]) {
                newExtrudable.GetComponent<Extrudable>().MakeAlreadyExtruded();
            }

            currentExtrudables.Add(newExtrudable);
        }
        puzzlesSolved = new bool[extrudableData.extrudableDataList.Count];
        ExtrudableList currentExtrudablesSet=extrudableData.extrudableDataList[currentExtrudableSetId];
        // Load in puzzle
        for (int i = 0; i < currentExtrudablesSet.extrudableSets.Count; i++) {
            // Instantiate destination blocks
            GameObject newDestination = Instantiate(extrudableData.destinationPrefab, currentExtrudablesSet.puzzle.puzzleBlocks[i].destinationPosition, Quaternion.identity);

            PuzzlePiece piece = new PuzzlePiece() {
                destinationObject = newDestination,
                correctSprite = currentExtrudablesSet.puzzle.puzzleBlocks[i].correctSprite,
                isCorrect = false
            };

            if (currentExtrudablesSet.puzzle.puzzleBlocks[i].isSolved) {
                newDestination.GetComponent<SpriteRenderer>().sprite = currentExtrudablesSet.puzzle.puzzleBlocks[i].correctSprite;
                piece.isCorrect = true;

                newDestination.name = currentExtrudablesSet.puzzle.puzzleBlocks[i].destinationName;
                newDestination.GetComponent<BlockScript>().blockId = i;
                newDestination.GetComponent<BlockScript>().blockName = currentExtrudablesSet.puzzle.puzzleBlocks[i].blockName;
            } else {
                newDestination.GetComponent<SpriteRenderer>().sprite = currentExtrudablesSet.puzzle.puzzleBlocks[i].destinationSprite;

                newDestination.name = currentExtrudablesSet.puzzle.puzzleBlocks[i].destinationName;
                newDestination.GetComponent<BlockScript>().blockId = i;
                newDestination.GetComponent<BlockScript>().blockName = currentExtrudablesSet.puzzle.puzzleBlocks[i].blockName;

                // Instantiate movable puzzle blocks
                GameObject newBlock = Instantiate(extrudableData.blockPrefab, currentExtrudablesSet.puzzle.puzzleBlocks[i].blockInitPosition, Quaternion.identity);

                newBlock.GetComponent<SpriteRenderer>().sprite = currentExtrudablesSet.puzzle.puzzleBlocks[i].blockSprite;
                newBlock.name = currentExtrudablesSet.puzzle.puzzleBlocks[i].blockName;
            }

            correctBlocks.Add(piece);
        }

        for (int k = 0; k < currentExtrudablesSet.puzzle.circuitSprites.Count; k++) {
            // Instantiate environment sprites
            GameObject newWire = Instantiate(extrudableData.circuitPrefab, currentExtrudablesSet.puzzle.circuitSprites[k].circuitInitPosition, Quaternion.identity);

            if ((k < currentExtrudablesSet.puzzle.puzzleBlocks.Count && currentExtrudablesSet.puzzle.puzzleBlocks[k].isSolved)|| currentExtrudablesSet.puzzle.circuitSprites[k].circuitName.Contains("env")) {
                newWire.GetComponent<SpriteRenderer>().sprite = currentExtrudablesSet.puzzle.circuitSprites[k].circuitSprite;
                newWire.name = currentExtrudablesSet.puzzle.circuitSprites[k].circuitName;
            } else {
                newWire.GetComponent<SpriteRenderer>().sprite = currentExtrudablesSet.puzzle.circuitSprites[k].circuitSprite;
                newWire.name = currentExtrudablesSet.puzzle.circuitSprites[k].circuitName;

                Color tmp = newWire.GetComponent<SpriteRenderer>().color;
                tmp.a = 0.3f;
                newWire.GetComponent<SpriteRenderer>().color = tmp;
            }
        }
    }

    void Update()
    {
        // If all extrudables are in their final state for current id (all isExtruding), return to the 3D scene
        bool allExtrudablesExtruded = true;
        foreach (GameObject extrudable in currentExtrudables) {
            if (!extrudable.GetComponent<Extrudable>().finishedExtruding) {
                allExtrudablesExtruded = false;
                break;
            }
        }

        if (allExtrudablesExtruded) {
           StartCoroutine(ExtrudableCompletionReturn());
        }
    }

    IEnumerator ExtrudableCompletionReturn()
    {
        yield return new WaitForSeconds(1);
        gameManager?.switchToScene("new3Dtut");
    }
}
