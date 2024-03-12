using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }
        
        // if (gameManager != null) {
        //     currentExtrudableSetId = gameManager.gameState.CurrentExtrudableSetId;
        // } else {
        //     currentExtrudableSetId = -1; // TODO: this should be -1 in the real game
        // }

        if (currentExtrudableSetId == -1) { // map should be blank, no puzzles loaded in
            Debug.Log("No extrudables selected");
            return;
        }

        List<ExtrudableData> extrudableSets = extrudableData.extrudableDataList[currentExtrudableSetId].extrudableSets;
        for (int i = 0; i < extrudableSets.Count; i++) {
            GameObject newExtrudable = Instantiate(extrudableData.extrudable2DPrefab, extrudableSets[i].position, Quaternion.identity);
            newExtrudable.transform.localScale = extrudableSets[i].scale;
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
