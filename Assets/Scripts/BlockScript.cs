using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    public PuzzleManager puzzleManager;
    public int blockId;
    public BoxCollider2D boxCollider;
    public Sprite connectedSprite;
    public GameObject blockPrefab;
    // Start is called before the first frame update
    void Start()
    {
        puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Block" && collision.gameObject.name == blockPrefab.name)
        {

            puzzleManager.correctBlocks[blockId] = new PuzzleManager.PuzzlePiece { position = puzzleManager.correctBlocks[blockId].position, correctTile = puzzleManager.correctBlocks[blockId].correctTile, isCorrect = true };
            Debug.Log("???");
            GameObject.Find("PuzzleManager").GetComponent<AudioSource>().Play();
        }
    }
}