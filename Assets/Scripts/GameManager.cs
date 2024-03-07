using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{   
    public GameState gameState = new GameState();
    public GameObject popupMenu;
    public static GameManager instance;
    private bool sceneLoaded = true;

    public float MusicVolume = 100f;
    public float DialogueVolume = 100f;
    public float SFXVolume = 100f;
    public InventorySystem inventorySystem;
    public PuzzleDataScriptable initTutorialPuzzle; // Initial puzzle states, loaded in via ScriptableObjects in the inspector
    public PuzzleDataScriptable initComputerPuzzle; // Initial puzzle states, loaded in via ScriptableObjects in the inspector
    public PuzzleDataScriptable initChemicalPuzzle;

    // Initial puzzle states, loaded in via ScriptableObjects in the inspector


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

        // Load in initial puzzle game data
        gameState.CurrTutorialPuzzle = Instantiate(initTutorialPuzzle);
        gameState.CurrComputerPuzzle = Instantiate(initComputerPuzzle);
        gameState.CurrChemicalPuzzle = Instantiate(initChemicalPuzzle);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {   
        Debug.Log("Scene loaded: " + scene.name);
        if (scene.name == "dialogue3DTut") {
            GameObject ThirdPersonCamera = GameObject.Find("Third Person Camera");
            ThirdPersonCamera.transform.position = gameState.CameraPosition3D;
            ThirdPersonCamera.transform.eulerAngles = gameState.CameraRotation3D;

            GameObject Player3D = GameObject.Find("3D Player");
            Player3D.transform.position = gameState.PlayerPosition3D;
            Player3D.transform.eulerAngles = gameState.PlayerRotation3D;

            if (gameState.PlayerHasUSB) {
                Debug.Log("Setting USB to inactive");
                GameObject.Find("USB").SetActive(false);
            }
            
            if (gameState.Door0Unlocked) {
                GameObject.Find("Door0").GetComponent<Animator>().SetBool("isOpen", true);
            }

            if (gameState.Door1Unlocked) {
                GameObject.Find("Door1").GetComponent<Animator>().SetBool("isOpen", true);
            }

            for (int i = 0; i < gameState.TutorialExtrudables.Count; i++) {
                if (gameState.TutorialExtrudables[i]) {
                    GameObject.Find("Extrudable" + i).GetComponent<Extrudable>().isMoving = true;
                }
            }
        }

        if (scene.name == "mainPuzzle") {
            // Note: puzzle manager will instantiate the relevant variables for us!
            PuzzleManager puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();

            puzzleManager.levelPuzzles = gameState.CurrTutorialPuzzle;

            // restore player position
            GameObject Player2D = GameObject.Find("2D Player");
            Player2D.transform.position = gameState.PlayerPuzzlePosition2D;
        }

        if (scene.name == "new2dtut") {
            // restore player position
            GameObject Player2D = GameObject.Find("2D Player");
            Player2D.transform.position = gameState.PlayerPosition2D;

            // Handle extrusion states
            foreach (GameObject extrudable in GameObject.FindGameObjectsWithTag("Extrudable")) {
                Extrudable ext = extrudable.GetComponent<Extrudable>();
                if (ext != null && gameState.TutorialExtrudables[ext.extrudableId]) {
                    ext.isMoving = true;
                }
            }
        }

        sceneLoaded = true;
    }
    

    void Update()
    {   
        if (!sceneLoaded) {
            return;
        }

        // Player State Persistence
        if (SceneManager.GetActiveScene().name == "dialogue3DTut") {
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

        if (SceneManager.GetActiveScene().name == "new2dtut") {
            GameObject Player2D = GameObject.Find("2D Player");
            if (Player2D != null) {
                gameState.PlayerPosition2D = Player2D.transform.position;
            }
        }

        // 2D Puzzle State Persistence
        if (SceneManager.GetActiveScene().name == "mainPuzzle") {
            GameObject Player2D = GameObject.Find("2D Player");
            if (Player2D != null) {
                gameState.PlayerPuzzlePosition2D = Player2D.transform.position;
            }

            // Update positions of each block in game state
            if (gameState.CurrentPuzzleId >= 0) {
                for (int i = 0; i < gameState.CurrTutorialPuzzle.puzzles[gameState.CurrentPuzzleId].puzzleBlocks.Count; i++) {
                    PuzzleSet temp = gameState.CurrTutorialPuzzle.puzzles[gameState.CurrentPuzzleId].puzzleBlocks[i];

                    if (!temp.isSolved) {
                        temp.blockInitPosition = GameObject.Find(temp.blockName).transform.position;
                        gameState.CurrTutorialPuzzle.puzzles[gameState.CurrentPuzzleId].puzzleBlocks[i] = temp;
                    }
                }
            }
        }

        // Scene Switch Logic
        if (Input.GetKeyDown(KeyCode.Q) && gameState.USBInserted) // M is cheat code to switch scenes
        {   
            Debug.Log("Switching scenes for level " + gameState.CurrentLevel);

            switch (gameState.CurrentLevel) {
                case Level.tutorial:

                    if (SceneManager.GetActiveScene().name == "dialogue3DTut")
                    {
                        sceneLoaded = false;
                        SceneManager.LoadScene("new2dtut");
                    }
                    else if (SceneManager.GetActiveScene().name == "new2dtut")
                    {
                        sceneLoaded = false;
                        SceneManager.LoadScene("dialogue3DTut");
                    }
                    else if (SceneManager.GetActiveScene().name == "mainPuzzle")
                    {
                        sceneLoaded = false;
                        SceneManager.LoadScene("dialogue3DTut");
                    }
                    break;
            }
        }

        // // C is a cheat code to try out the computer puzzle
        // if (Input.GetKeyDown(KeyCode.C)) {
        //     gameState.CurrentLevel = Level.computerlab;
        //     sceneLoaded = false;
        //     gameState.CurrentPuzzleId = 0;
        //     SceneManager.LoadScene("computerPuzzle");
        // }

        // // V is a cheat code to view the monitor room
        // if (Input.GetKeyDown(KeyCode.V) && SceneManager.GetActiveScene().name == "fbx3dmain") {
        //     GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(2.90f,11.88f,-11.48f);
        // }

        // // X is a cheat code to extrude everything
        // if (Input.GetKeyDown(KeyCode.X)) {
        //     foreach (GameObject ext in GameObject.FindGameObjectsWithTag("Extrudable")) {
        //         ext.GetComponent<Extrudable>().Extrude();
        //     }
        // }
    }

    public void SwitchToPuzzle(int puzzleId) {
        gameState.CurrentPuzzleId = puzzleId;
        sceneLoaded = false;
        SceneManager.LoadScene("mainPuzzle");
    }

    public void SetPlayerPosition(Vector3 position) {
        gameState.PlayerPosition3D = position;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePauseMenu();
        }
    }

    public void GetUSB() {
        gameState.PlayerHasUSB = true;
        inventorySystem.AddObjectToInventory();
        GameObject.FindGameObjectWithTag("USB").SetActive(false);
    }

    public void InsertUSB() {
        if (gameState.PlayerHasUSB) {
            gameState.USBInserted = true;
        } else {
            print("Player does not have USB");
        }
    }

    public void UpdateExtrudables(int extrudableId) {
        switch (gameState.CurrentLevel) {
            case Level.tutorial:
                gameState.TutorialExtrudables[extrudableId] = true;
                break;
            case Level.biolab:
                gameState.BioLabExtrudables[extrudableId] = true;
                break;
            case Level.computerlab:
                gameState.ComputerLabExtrudables[extrudableId] = true;
                break;
        }
    }

    public void SolvePuzzleBlock(int puzzleId, int blockId, Level level) {
        switch (level) {
            case Level.tutorial:
                PuzzleSet x = gameState.CurrTutorialPuzzle.puzzles[puzzleId].puzzleBlocks[blockId];
                x.isSolved = true;
                gameState.CurrTutorialPuzzle.puzzles[puzzleId].puzzleBlocks[blockId] = x;
                break;
            case Level.biolab:
                PuzzleSet y = gameState.CurrComputerPuzzle.puzzles[puzzleId].puzzleBlocks[blockId];
                y.isSolved = true;
                gameState.CurrComputerPuzzle.puzzles[puzzleId].puzzleBlocks[blockId] = y;
                break;
            case Level.computerlab:
                PuzzleSet z = gameState.CurrChemicalPuzzle.puzzles[puzzleId].puzzleBlocks[blockId];
                z.isSolved = true;
                gameState.CurrChemicalPuzzle.puzzles[puzzleId].puzzleBlocks[blockId] = z;
                break;
        }
    }

    public void SolvePuzzle(Level level, int puzzleId) {
        switch (level) {
            case Level.tutorial:
                switch (puzzleId) {
                    case 0: 
                        gameState.Door0Unlocked = true;
                        break;
                    case 1:
                        gameState.Door1Unlocked = true;
                        break;
                }
                break;
            case Level.biolab:
                break;
            case Level.computerlab:
                break;
        }
    }

    public void SetCurrentPuzzle(int puzzleId) {
        gameState.CurrentPuzzleId = puzzleId;
    }

    // Set to new level and switch scene if need be
    public void SetCurrentLevel(Level level) {
        gameState.CurrentLevel = level;
        gameState.CurrentPuzzleId = 0;

        switch (level) {
            case Level.biolab:
                sceneLoaded = false;
                SceneManager.LoadScene("fbx3dmain");
                break;
            // case Level.computerlab:
            //     gameState.CurrChemicalPuzzle = Instantiate(initChemicalPuzzle);
            //     break;
        }
    }

    // public void ActivateChemPuzzle() {
    //     gameState.ChemPuzzleUnlocked = true;
    //     gameState.CurrentPuzzleId = 0;
    //     gameState.CurrentLevel = Level.biolab;

    //     sceneLoaded = false;
    //     SceneManager.LoadScene("chemicalPuzzle");
    // }

    // public void ActivateComputerPuzzle() {
    //     gameState.ComputerPuzzleUnlocked = true;
    //     gameState.CurrentPuzzleId = 0;
    //     gameState.CurrentLevel = Level.computerlab;

    //     sceneLoaded = false;
    //     SceneManager.LoadScene("computerPuzzle");
    // }

    public void TogglePauseMenu()
    {
        if (popupMenu.activeSelf)
        {
            popupMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            popupMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}

public class GameState
{
    // Current level
    public Level CurrentLevel { get; set; }
    public int CurrentPuzzleId { get; set; } // id of puzzle in current level

    // 3D Character State
    public Vector3 PlayerPosition3D { get; set; }
    public Vector3 PlayerRotation3D { get; set; }
    public Vector3 CameraPosition3D { get; set; }
    public Vector3 CameraRotation3D { get; set; }

    // 3D Game State
    // Tutorial
    public bool Door0Unlocked { get; set; }
    public bool Door1Unlocked { get; set; }
    public bool PlayerHasUSB { get; set; }
    public bool USBInserted { get; set; }

    // Biolab
    // public bool ChemPuzzleUnlocked { get; set; }
    // public bool ComputerPuzzleUnlocked { get; set; }
    
    // 2D Character State
    public Vector2 PlayerPosition2D { get; set; }
    public Vector2 PlayerPuzzlePosition2D { get; set; }

    // 2D Game State
    public PuzzleDataScriptable CurrTutorialPuzzle { get; set; }
    public PuzzleDataScriptable CurrChemicalPuzzle { get; set; }
    public PuzzleDataScriptable CurrComputerPuzzle { get; set; }

    // Lists of bools indicating whether or not the corresponding extrudable block in that level has been extruded
    public List<bool> TutorialExtrudables { get; set; }
    public List<bool> BioLabExtrudables { get; set; }
    public List<bool> ComputerLabExtrudables { get; set; }

    // Scene State
    public string SceneName { get; set; }

    // Constructor
    public GameState()
    {
        CurrentLevel = Level.tutorial;
        CurrentPuzzleId = -1;

        PlayerPosition3D = new Vector3(3f,4.28f,24.2f);
        PlayerRotation3D = new Vector3(0f,180f,0f);
        CameraPosition3D = Vector3.zero;
        CameraRotation3D = Vector3.zero;

        Door0Unlocked = false;
        Door1Unlocked = false;
        PlayerHasUSB = false;
        USBInserted = false;

        PlayerPosition2D = new Vector3(0.13f, 2.1f, 0.0f);
        PlayerPuzzlePosition2D = new Vector3(-6.64f,-1.75f,0f);

        TutorialExtrudables = new List<bool> { false, false, false };
        BioLabExtrudables = new List<bool> { false, false, false, false, false };
        ComputerLabExtrudables = new List<bool> { false, false, false };

        SceneName = "dialogue3DTut";
    }
}
