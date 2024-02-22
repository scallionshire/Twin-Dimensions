using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{   
    public GameState gameState = new GameState();
    public static GameManager instance;
    private bool sceneLoaded = true;

    void Awake()
    {   
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {   
        Debug.Log("Scene loaded: " + scene.name);
        if (scene.name == "updated3DTut") {
            GameObject ThirdPersonCamera = GameObject.Find("Third Person Camera");
            ThirdPersonCamera.transform.position = gameState.CameraPosition3D;
            ThirdPersonCamera.transform.eulerAngles = gameState.CameraRotation3D;

            if (gameState.PlayerHasUSB) {
                Debug.Log("Setting USB to inactive");
                GameObject.Find("USB").SetActive(false);
            }
            
            if (gameState.DoorUnlocked) {
                GameObject.Find("Door").GetComponent<Animator>().SetBool("isOpen", true);

                // Enable the moving platforms
                GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
                Debug.Log("Platform moving");
                foreach (GameObject platform in platforms) {
                    platform.GetComponent<Extrudable>().isMoving = true;
                }
            }

            if (gameState.WallPushedIn) {
                GameObject.Find("ExtrudeWall").GetComponent<Extrudable>().isMoving = true;
            }
        }

        if (scene.name == "updated2dTut") {
            PuzzleManager puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();

            GameObject PushWall = GameObject.Find("PushWall");
            if (gameState.WallPushedIn) {
                PushWall.transform.position = gameState.PushWallPosition;
                PushWall.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }

            GameObject ElevatorBlock = GameObject.Find("elevator");
            if (ElevatorBlock != null) 
            {
                if (gameState.ElevatorBlockSet) {
                    Destroy(ElevatorBlock);
                    GameObject.Find("elevatorTrigger").GetComponent<SpriteRenderer>().sprite = puzzleManager.correctBlocks[0].correctSprite;
                } else {
                    Debug.Log("Setting ElevatorBlock to position " + gameState.ElevatorBlockPosition);
                    ElevatorBlock.transform.localPosition = gameState.ElevatorBlockPosition;
                }
            }

            GameObject PinkBlock = GameObject.Find("pink");
            if (PinkBlock != null) 
            {
                if (gameState.PinkBlockSet) {
                    Destroy(PinkBlock);
                    GameObject.Find("pinkTrigger").GetComponent<SpriteRenderer>().sprite = puzzleManager.correctBlocks[2].correctSprite;
                } else {
                    Debug.Log("Setting PinkBlock to position " + gameState.PinkBlockPosition);
                    PinkBlock.transform.localPosition = gameState.PinkBlockPosition;
                }
            }

            GameObject GreenBlock = GameObject.Find("green");
            if (GreenBlock != null) 
            {
                if (gameState.GreenBlockSet) {
                    Destroy(GreenBlock);
                    GameObject.Find("greenTrigger").GetComponent<SpriteRenderer>().sprite = puzzleManager.correctBlocks[1].correctSprite;
                } else {
                    Debug.Log("Setting GreenBlock to position " + gameState.GreenBlockPosition);
                    GreenBlock.transform.localPosition = gameState.GreenBlockPosition;
                }
            }

            GameObject YellowBlock = GameObject.Find("yellow");
            if (YellowBlock != null) 
            {
                if (gameState.YellowBlockSet) {
                    Destroy(YellowBlock);
                    GameObject.Find("yellowTrigger").GetComponent<SpriteRenderer>().sprite = puzzleManager.correctBlocks[3].correctSprite;
                } else {
                    Debug.Log("Setting YellowBlock to position " + gameState.YellowBlockPosition);
                    YellowBlock.transform.localPosition = gameState.YellowBlockPosition;
                }
            }

            for (int blockId = 0; blockId < 4; blockId++) {
                if (gameState.completedBlocks.Contains(blockId)) {
                    if (puzzleManager != null) {
                        puzzleManager.correctBlocks[blockId] = new PuzzleManager.PuzzlePiece { destinationObject = puzzleManager.correctBlocks[blockId].destinationObject, correctSprite = puzzleManager.correctBlocks[blockId].correctSprite, isCorrect = true };
                    }
                }
            }

            // restore player position
            GameObject Player2D = GameObject.Find("2D Player");
            Player2D.transform.position = gameState.PlayerPosition2D;
        }

        sceneLoaded = true;
    }

    void Update()
    {   
        if (!sceneLoaded) {
            return;
        }
        // Player State Persistence
        if (SceneManager.GetActiveScene().name == "updated3DTut") {
            GameObject Player3D = GameObject.Find("3D Player");
            if (Player3D != null) {
                gameState.PlayerPosition3D = Player3D.transform.position;
                gameState.PlayerRotation3D = Player3D.transform.eulerAngles;
            }

            GameObject ThirdPersonCamera = GameObject.Find("Third Person Camera");
            if (ThirdPersonCamera != null) {
                gameState.CameraPosition3D = ThirdPersonCamera.transform.position;
                gameState.CameraRotation3D = ThirdPersonCamera.transform.eulerAngles;
            }
        }

        // 2D Puzzle State Persistence
        if (SceneManager.GetActiveScene().name == "updated2dTut") {
            GameObject PushWall = GameObject.Find("PushWall");
            if (!gameState.WallPushedIn) {
                gameState.PushWallPosition = PushWall.transform.position;
            }

            GameObject ElevatorBlock = GameObject.Find("elevator");
            if (ElevatorBlock != null) {
                gameState.ElevatorBlockPosition = ElevatorBlock.transform.localPosition;
            }
            GameObject PinkBlock = GameObject.Find("pink");
            if (PinkBlock != null) {
                gameState.PinkBlockPosition = PinkBlock.transform.localPosition;
            }
            GameObject GreenBlock = GameObject.Find("green");
            if (GreenBlock != null) {
                gameState.GreenBlockPosition = GreenBlock.transform.localPosition;
            }
            GameObject YellowBlock = GameObject.Find("yellow");
            if (YellowBlock != null) {
                gameState.YellowBlockPosition = YellowBlock.transform.localPosition;
            }
            GameObject Player2D = GameObject.Find("2D Player");
            if (Player2D != null) {
                gameState.PlayerPosition2D = Player2D.transform.position;
            }
        }

        // Scene Switch Logic
        if (Input.GetKeyDown(KeyCode.Q) && gameState.USBInserted || Input.GetKeyDown(KeyCode.M)) // M is cheat code to switch scenes
        {   
            Debug.Log("Switching scenes");
            if (SceneManager.GetActiveScene().name == "updated2dTut")
            {
                sceneLoaded = false;
                SceneManager.LoadScene("updated3DTut");
            }
            else if (SceneManager.GetActiveScene().name == "updated3DTut")
            {
                sceneLoaded = false;
                SceneManager.LoadScene("updated2dTut");
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
    public bool WallPushedIn { get; set; }
    
    // 2D Character State
    public Vector2 PlayerPosition2D { get; set; }

    // 2D Game State
    public Vector3 PushWallPosition { get; set; }
    public Vector3 ElevatorBlockPosition { get; set; }
    public Vector3 PinkBlockPosition { get; set; }
    public Vector3 GreenBlockPosition { get; set; }
    public Vector3 YellowBlockPosition { get; set; }
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
        WallPushedIn = false;

        PlayerPosition2D = new Vector3(0.13f, 2.1f, 0.0f);

        PushWallPosition = new Vector3(-1.2622f, -0.2348f, 0.0f);
        ElevatorBlockPosition = new Vector3(0.035f, 0.525f, 0.0f);
        PinkBlockPosition = new Vector3(-2.5f, 1.5f, 0.0f);
        YellowBlockPosition = new Vector3(0.5f, -0.7f, 0.0f);
        GreenBlockPosition = new Vector3(-6.5f, 0.5f, 0.0f);
        ElevatorBlockSet = false;
        PinkBlockSet = false;
        GreenBlockSet = false;
        YellowBlockSet = false;
    }
}
