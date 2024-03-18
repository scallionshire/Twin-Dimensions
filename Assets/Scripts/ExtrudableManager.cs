using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ExtrudableManager : MonoBehaviour
{
    public int currentExtrudableSetId;
    public ExtrudableDataScriptable extrudableData;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("GameManager") != null) {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            currentExtrudableSetId = gameManager.gameState.CurrentExtrudableSetId;
        }

        if (currentExtrudableSetId == -1) { // map should be blank, no puzzles loaded in
            Debug.Log("No extrudables selected");
            return;
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

            if (extrudableSets[i].alreadyExtruded) {
                newExtrudable.GetComponent<Extrudable>().MakeAlreadyExtruded();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
