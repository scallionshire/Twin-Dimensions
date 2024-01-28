using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{   
    private bool hasUSB = false;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>(); // Get the GameManager object and its GameManager component
        Debug.Log("Loaded player position is " + gameManager.playerPosition);
        transform.position = gameManager.playerPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(hasUSB);
    }

    public void setHasUSB(bool hasUSB)
    {
        this.hasUSB = hasUSB;
    }
    
    public bool getHasUSB()
    {
        return hasUSB;
    }

    public void savePlayer()
    {   
        Debug.Log("Saving player position");
        gameManager.playerPosition = transform.position;
        gameManager.doorUnlocked = true;
    }
}
