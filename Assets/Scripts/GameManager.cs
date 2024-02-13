using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    public GameState gameState = new GameState();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {   
        Debug.Log("Scene loaded: " + scene.name);
        if (scene.name == "tut3d") {
            GameObject ThirdPersonCamera = GameObject.Find("Third Person Camera");
            ThirdPersonCamera.transform.position = gameState.CameraPosition3D;
            ThirdPersonCamera.transform.eulerAngles = gameState.CameraRotation3D;

            if (gameState.PlayerHasUSB) {
                Debug.Log("Setting USB to inactive");
                GameObject.Find("USB").SetActive(false);
            }
            
            if (gameState.DoorUnlocked) {
                // TODO: replace the above with a door anim instead
                GameObject.Find("Door").GetComponent<Animator>().SetBool("isOpen", true);

                // Enable the moving platforms
                GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
                Debug.Log("Platform moving");
                foreach (GameObject platform in platforms) {
                    platform.GetComponent<Extrudable>().isExtruding = true;
                }
            }
        }

        if (scene.name == "tut2d") {
            
            // restore all block to stored positions
            Debug.Log("Restoring block positions");
            GameObject ElevatorBlock = GameObject.Find("elevator");
            if (ElevatorBlock != null) 
            {
                if (gameState.ElevatorBlockSet) {
                    Destroy(ElevatorBlock);
                } else {
                    ElevatorBlock.transform.position = gameState.ElevatorBlockPosition;
                }
            }

            GameObject PinkBlock = GameObject.Find("pink");
            if (PinkBlock != null) 
            {
                if (gameState.PinkBlockSet) {
                    Destroy(PinkBlock);
                } else {
                    PinkBlock.transform.position = gameState.PinkBlockPosition;
                }
            }

            GameObject GreenBlock = GameObject.Find("green");
            if (GreenBlock != null) 
            {
                if (gameState.GreenBlockSet) {
                    Destroy(GreenBlock);
                } else {
                    GreenBlock.transform.position = gameState.GreenBlockPosition;
                }
            }

            GameObject YellowBlock = GameObject.Find("yellow");
            if (YellowBlock != null) 
            {
                if (gameState.YellowBlockSet) {
                    Destroy(YellowBlock);
                } else {
                    YellowBlock.transform.position = gameState.YellowBlockPosition;
                }
            }

            for (int blockId = 0; blockId < 4; blockId++) {
                if (gameState.completedBlocks.Contains(blockId)) {
                    PuzzleManager puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
                    if (puzzleManager != null) {
                        puzzleManager.correctBlocks[blockId] = new PuzzleManager.PuzzlePiece { destinationObject = puzzleManager.correctBlocks[blockId].destinationObject, isCorrect = true };
                    }
                }
            }

            // restore player position
            GameObject Player2D = GameObject.Find("2D Player");
            Player2D.transform.position = gameState.PlayerPosition2D;
        }
    }

    void Update()
    {   
        // Player State Persistence
        GameObject Player3D = GameObject.Find("3D Player");
        if (Player3D != null) {
            gameState.PlayerPosition3D = Player3D.transform.position;
            gameState.PlayerRotation3D = Player3D.transform.eulerAngles;
        }

        GameObject Player2D = GameObject.Find("2D Player");
        if (Player2D != null) {
            gameState.PlayerPosition2D = Player2D.transform.position;
        }

        GameObject ThirdPersonCamera = GameObject.Find("Third Person Camera");
        if (ThirdPersonCamera != null) {
            gameState.CameraPosition3D = ThirdPersonCamera.transform.position;
            gameState.CameraRotation3D = ThirdPersonCamera.transform.eulerAngles;
        }

        // 2D Puzzle State Persistence
        GameObject ElevatorBlock = GameObject.Find("elevator");
        if (ElevatorBlock != null) {
            gameState.ElevatorBlockPosition = ElevatorBlock.transform.position;
        }
        GameObject PinkBlock = GameObject.Find("pink");
        if (PinkBlock != null) {
            gameState.PinkBlockPosition = PinkBlock.transform.position;
        }
        GameObject GreenBlock = GameObject.Find("green");
        if (GreenBlock != null) {
            gameState.GreenBlockPosition = GreenBlock.transform.position;
        }
        GameObject YellowBlock = GameObject.Find("yellow");
        if (YellowBlock != null) {
            gameState.YellowBlockPosition = YellowBlock.transform.position;
        }

        // Scene Switch Logic
        if (Input.GetKeyDown(KeyCode.Q) && gameState.USBInserted)
        {   
            Debug.Log("Switching scenes");
            if (SceneManager.GetActiveScene().name == "tut2d")
            {
                SceneManager.LoadScene("tut3d");
            }
            else if (SceneManager.GetActiveScene().name == "tut3d")
            {
                SceneManager.LoadScene("tut2d");
            }
        }
    }
}

public class GameState
{
    // 3D Character State
    public Vector3 PlayerPosition3D { get; set; }
    public Vector3 PlayerRotation3D { get; set; }
    public Vector3 CameraPosition3D { get; set; }
    public Vector3 CameraRotation3D { get; set; }

    // 3D Game State
    public bool DoorUnlocked { get; set; }
    public bool PlayerHasUSB { get; set; }
    public bool USBInserted { get; set; }
    
    // 2D Character State
    public Vector2 PlayerPosition2D { get; set; }

    // 2D Game State
    public Vector2 ElevatorBlockPosition { get; set; }
    public Vector2 PinkBlockPosition { get; set; }
    public Vector2 GreenBlockPosition { get; set; }
    public Vector2 YellowBlockPosition { get; set; }
    public bool ElevatorBlockSet { get; set; }
    public bool PinkBlockSet { get; set; }
    public bool GreenBlockSet { get; set; }
    public bool YellowBlockSet { get; set; }
    public List<int> completedBlocks = new List<int>();

    // Scene State
    public string SceneName { get; set; }

    // Constructor
    public GameState()
    {
        PlayerPosition3D = new Vector3(-0.73f,3.877926f,27.88f);
        PlayerRotation3D = new Vector3(0f,90f,0f);
        CameraPosition3D = Vector3.zero;
        CameraRotation3D = Vector3.zero;

        DoorUnlocked = false;
        PlayerHasUSB = false;
        USBInserted = false;

        PlayerPosition2D = Vector2.zero;

        ElevatorBlockPosition = new Vector2(-1.5f, 0.75f);
        PinkBlockPosition = new Vector2(0.5f, -1.3f);
        YellowBlockPosition = new Vector2(-0.5f, -1.3f);
        GreenBlockPosition = new Vector2(0.5f, 1.1f);
        ElevatorBlockSet = false;
        PinkBlockSet = false;
        GreenBlockSet = false;
        YellowBlockSet = false;
    }
}
