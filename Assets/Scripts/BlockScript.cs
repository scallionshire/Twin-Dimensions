using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    public PuzzleManager puzzleManager;
    public int blockId;
    public BoxCollider2D boxCollider;
    public string blockName;
    private AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        boxCollider = GetComponent<BoxCollider2D>();
        if (GameObject.Find("GameManager") != null) {
            audioManager = GameObject.Find("GameManager").GetComponent<AudioManager>();
        }
    }

    // Update is called once per frame

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Block" && collision.gameObject.name == blockName)
        {
            puzzleManager.correctBlocks[blockId] = new PuzzleManager.PuzzlePiece { destinationObject = puzzleManager.correctBlocks[blockId].destinationObject, correctSprite = puzzleManager.correctBlocks[blockId].correctSprite, isCorrect = true };

            if (audioManager != null) {
                audioManager.Play("Success");
            }

            Destroy(collision.gameObject);
        }
    }
}