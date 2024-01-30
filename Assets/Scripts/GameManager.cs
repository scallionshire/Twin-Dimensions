using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    public Vector3 playerPosition;
    public bool doorUnlocked;

    void Awake()
    {
        DontDestroyOnLoad (gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loaded scene: " + scene.name);
        if (doorUnlocked && scene.name == "tutroom") {
            Destroy(GameObject.Find("Door")); 
            // TODO: replace the above with a door anim instead

            // Enable the moving platforms
            GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");

            foreach (GameObject platform in platforms) {
                platform.GetComponent<PlatformMovement>().isMoving = true;
            }
        }  
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
