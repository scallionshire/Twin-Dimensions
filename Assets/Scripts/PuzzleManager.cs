using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleManager : MonoBehaviour
{
    [System.Serializable]
    public struct PuzzlePiece {
        public Vector3Int position;
        public TileBase correctTile;
        public bool isCorrect;
    }

    public Tilemap tilemap;
    public TileBase startTileDone;
    public TileBase endTileDone;
    public List<PuzzlePiece> correctBlocks;
    private AudioManager audioManager;

    void Start()
    {
        tilemap = GameObject.Find("Tilemap_Blocks").GetComponent<Tilemap>();
        //audioManager = GameObject.Find("GameManager").GetComponent<AudioManager>();
    }

    void Update()
    {
        // Check for puzzle completion
        if (IsPuzzleSolved())
        {
            // Puzzle is solved, provide feedback and handle progression
            //StartCoroutine(changeScene());
             Debug.Log("Puzzle solved");
        }
    }

    IEnumerator changeScene()
    {
        tilemap.SetTile(new Vector3Int(-3, 1, 0), startTileDone);
        tilemap.SetTile(new Vector3Int(1, -1, 0), startTileDone);
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 180f, 0f), Vector3.one);
        tilemap.SetTransformMatrix(new Vector3Int(1, -1, 0), matrix);
        yield return new WaitForSeconds(2);
        GameObject.Find("GameManager").GetComponent<GameManager>().doorUnlocked = true;
        audioManager.Play("DoorOpen");
        SceneManager.LoadScene("SampleScene 1");
    }

    bool IsPuzzleSolved()
    {
        // Iterate through each cell in the tilemap
        foreach (PuzzlePiece piece in correctBlocks)
        {
             Debug.Log("Piece: " + piece.position + " - IsCorrect: " + piece.isCorrect);
            if (!piece.isCorrect)
            {
                // Check for the right tile here
                return false;
            } 
            else 
            {
                tilemap.SetTile(piece.position, piece.correctTile);
            }
        }

        return true;
    }
}