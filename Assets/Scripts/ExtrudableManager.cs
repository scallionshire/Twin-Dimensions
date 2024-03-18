using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrudableManager : MonoBehaviour
{
    public int currentExtrudableSetId;
    public ExtrudableDataScriptable extrudableData;

    private GameManager gameManager;

    // Start is called before the first frame update
    public void LoadMap()
    {
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

        List<ExtrudableData> extrudableSets = extrudableData.extrudableDataList[currentExtrudableSetId].extrudableSets;
        for (int i = 0; i < extrudableSets.Count; i++) {
            GameObject newExtrudable = Instantiate(extrudableData.extrudable2DPrefab, extrudableSets[i].position, Quaternion.identity);
            newExtrudable.GetComponent<SpriteRenderer>().size = extrudableSets[i].size;
            newExtrudable.GetComponent<BoxCollider2D>().size = extrudableSets[i].size;
            newExtrudable.transform.rotation = Quaternion.Euler(extrudableSets[i].rotation);
            newExtrudable.name = "Extrudable" + extrudableSets[i].id;

            newExtrudable.GetComponent<Extrudable>().extrudableId = extrudableSets[i].id;
            newExtrudable.GetComponent<Extrudable>().extrudeDirection = extrudableSets[i].direction;
            newExtrudable.GetComponent<Extrudable>().extrudeAmount = extrudableSets[i].amount;
            newExtrudable.GetComponent<Extrudable>().shouldLoop = extrudableSets[i].shouldLoop;
            newExtrudable.GetComponent<Extrudable>().isExtruding = extrudableSets[i].shouldExtrude;

            if (gameManager != null && gameManager.gameState.Extrudables[extrudableSets[i].id]) {
                newExtrudable.GetComponent<Extrudable>().MakeAlreadyExtruded();
            }
        }
    }
}
