using UnityEngine;

public class BlockScript : MonoBehaviour
{
    public int blockId;
    public string blockName;

    private PuzzleManager puzzleManager;

    // Start is called before the first frame update
    void Start()
    {
        puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
    }

    // Update is called once per frame

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Block" && collision.gameObject.name == blockName)
        {
            puzzleManager.correctBlocks[blockId] = new PuzzlePiece { destinationObject = puzzleManager.correctBlocks[blockId].destinationObject, correctSprite = puzzleManager.correctBlocks[blockId].correctSprite, isCorrect = true };
        }
    }
}