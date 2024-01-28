using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    public Vector3 playerPosition;
    public bool doorUnlocked;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject); // Don't destroy this object when loading a new scene
    }

    // Update is called once per frame
    void Update()
    {        
        if (doorUnlocked) {
            Destroy(GameObject.Find("Door")); 
        }
    }
}
