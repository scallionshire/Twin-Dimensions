using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{    // Start is called before the first frame update
    void Start()
    {   
        transform.position = GameManager.instance.gameState.PlayerPosition3D;
        transform.rotation = Quaternion.Euler(GameManager.instance.gameState.PlayerRotation3D);
    }
}
