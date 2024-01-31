using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {   
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        transform.position = gameManager.gameState.PlayerPosition3D;
        transform.rotation = Quaternion.Euler(gameManager.gameState.PlayerRotation3D);
    }
}
