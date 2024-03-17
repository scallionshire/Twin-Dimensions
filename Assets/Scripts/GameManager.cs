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

    public ExtrudableDataScriptable initTutorialExtrudables;
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

        // Doing this prevents losing reference to the popup menu
        TogglePauseMenu();
        TogglePauseMenu();

        // Load in initial puzzle game data
        gameState.CurrTutorialPuzzle = Instantiate(initTutorialPuzzle);
        gameState.CurrComputerPuzzle = Instantiate(initComputerPuzzle);
        gameState.CurrChemicalPuzzle = Instantiate(initChemicalPuzzle);
        gameState.CurrTutorialExtrudables = Instantiate(initTutorialExtrudables);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {   
        Debug.Log("Scene loaded: " + scene.name);

        if (scene.name == "new3Dtut") {
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

            // for (int i = 0; i < gameState.TutorialExtrudables.Count; i++) {
            //     Debug.Log("Checking extrudable: " + i + gameState.TutorialExtrudables[i]);
            //     if (gameState.TutorialExtrudables[i]) {
            //         GameObject.Find("Extrudable" + i).GetComponent<Extrudable>().MakeAlreadyExtruded();
            //     }
            // }

            Cursor.lockState = CursorLockMode.Locked;
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

            ExtrudableManager extrudableManager = GameObject.Find("ExtrudableManager").GetComponent<ExtrudableManager>();
            extrudableManager.currentExtrudableSetId = gameState.CurrentExtrudableSetId;
            extrudableManager.extrudableData = gameState.CurrTutorialExtrudables;

            // Handle extrusion states
            // foreach (GameObject extrudable in GameObject.FindGameObjectsWithTag("Extrudable")) {
            //     Extrudable ext = extrudable.GetComponent<Extrudable>();
            //     if (ext != null && gameState.TutorialExtrudables[ext.extrudableId]) {
            //         ext.MakeAlreadyExtruded();
            //     }
            // }
        }

        sceneLoaded = true;
    }
    

    void Update()
    {   
        if (!sceneLoaded) {
            return;
        }

        if (SceneManager.GetActiveScene().name == "StartMenu") {
            Cursor.lockState = CursorLockMode.None;
        }

        // Player State Persistence
        if (SceneManager.GetActiveScene().name == "new3Dtut") {
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

            if (gameState.CurrentExtrudableSetId >= 0) {
                for (int i = 0; i < gameState.CurrTutorialExtrudables.extrudableDataList[gameState.CurrentExtrudableSetId].extrudableSets.Count; i++) {
                    ExtrudableData temp = gameState.CurrTutorialExtrudables.extrudableDataList[gameState.CurrentExtrudableSetId].extrudableSets[i];
                    if (gameState.TutorialExtrudables[temp.id]) {
                        Debug.Log("Setting extrudable to already extruded: " + temp.id);
                        temp.alreadyExtruded = true;
                        gameState.CurrTutorialExtrudables.extrudableDataList[gameState.CurrentExtrudableSetId].extrudableSets[i] = temp;
                    }
                }
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

                    if (SceneManager.GetActiveScene().name == "new3Dtut")
                    {
                        sceneLoaded = false;
                        SceneManager.LoadScene("new2dtut");
                    }
                    else if (SceneManager.GetActiveScene().name == "new2dtut")
                    {
                        sceneLoaded = false;
                        SceneManager.LoadScene("new3Dtut");
                    }
                    else if (SceneManager.GetActiveScene().name == "mainPuzzle")
                    {
                        sceneLoaded = false;
                        SceneManager.LoadScene("new3Dtut");
                    }
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePauseMenu();
        }
    }

    public void SwitchToPuzzle(int puzzleId) {
        instance.gameState.CurrentPuzzleId = puzzleId;
        sceneLoaded = false;
        SceneManager.LoadScene("mainPuzzle");
    }

    public void SwitchToMap(int setId) {
        instance.gameState.CurrentExtrudableSetId = setId;
        sceneLoaded = false;
        SceneManager.LoadScene("new2dtut");
    }

    public void SetPlayerPosition(Vector3 position) {
        instance.gameState.PlayerPosition3D = position;
    }

    public void GetUSB() {
        instance.gameState.PlayerHasUSB = true;
        // gameState.PlayerHasUSB = true;
        if (GameObject.Find("InventorySystem") != null) {
            Debug.Log("Adding USB to inventory");
            InventorySystem inventorySystem = GameObject.Find("InventorySystem").GetComponent<InventorySystem>();
            inventorySystem.AddObjectToInventory();
        }
        GameObject.FindGameObjectWithTag("USB").SetActive(false);
    }

    public void InsertUSB() {
        if (instance.gameState.PlayerHasUSB) {
            instance.gameState.USBInserted = true;
            // gameState.USBInserted = true;
            Debug.Log("Game state, USB inserted: " + instance.gameState.USBInserted);
        } else {
            Debug.Log("Player does not have USB");
        }
        if (GameObject.Find("TooltipCanvas") != null)
        {
            GameObject.Find("TooltipCanvas").GetComponent<TooltipManager>().ShowQTooltip();
        }
    }

    public void UpdateExtrudables(int extrudableId) {
        switch (gameState.CurrentLevel) {
            case Level.tutorial:
                Debug.Log("Updating extrudable: " + extrudableId);
                instance.gameState.TutorialExtrudables[extrudableId] = true;
                break;
            case Level.biolab:
                instance.gameState.BioLabExtrudables[extrudableId] = true;
                break;
            case Level.computerlab:
                instance.gameState.ComputerLabExtrudables[extrudableId] = true;
                break;
        }
    }

    public void SolvePuzzleBlock(int puzzleId, int blockId, Level level) {
        switch (level) {
            case Level.tutorial:
                PuzzleSet x = instance.gameState.CurrTutorialPuzzle.puzzles[puzzleId].puzzleBlocks[blockId];
                x.isSolved = true;
                instance.gameState.CurrTutorialPuzzle.puzzles[puzzleId].puzzleBlocks[blockId] = x;
                break;
            case Level.biolab:
                PuzzleSet y = instance.gameState.CurrComputerPuzzle.puzzles[puzzleId].puzzleBlocks[blockId];
                y.isSolved = true;
                instance.gameState.CurrComputerPuzzle.puzzles[puzzleId].puzzleBlocks[blockId] = y;
                break;
            case Level.computerlab:
                PuzzleSet z = instance.gameState.CurrChemicalPuzzle.puzzles[puzzleId].puzzleBlocks[blockId];
                z.isSolved = true;
                instance.gameState.CurrChemicalPuzzle.puzzles[puzzleId].puzzleBlocks[blockId] = z;
                break;
        }
    }

    public void SolvePuzzle(Level level, int puzzleId) {
        switch (level) {
            case Level.tutorial:
                switch (puzzleId) {
                    case 0: 
                        instance.gameState.Door0Unlocked = true;
                        break;
                    case 1:
                        instance.gameState.Door1Unlocked = true;
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
        instance.gameState.CurrentPuzzleId = puzzleId;
    }

    // Set to new level and switch scene if need be
    public void SetCurrentLevel(Level level) {
        instance.gameState.CurrentLevel = level;
        instance.gameState.CurrentPuzzleId = 0;

        switch (level) {
            case Level.tutorial:
                sceneLoaded = false;
                SceneManager.LoadScene("fbx3dmain");
                break;
            // case Level.computerlab:
            //     gameState.CurrChemicalPuzzle = Instantiate(initChemicalPuzzle);
            //     break;
        }
    }

    public void TogglePauseMenu()
    {
        if (popupMenu.activeSelf)
        {
            popupMenu.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
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
    public int CurrentExtrudableSetId { get; set; }

    public ExtrudableDataScriptable CurrTutorialExtrudables { get; set; }
    public List<bool> TutorialExtrudables { get; set; }
    public List<bool> BioLabExtrudables { get; set; }
    public List<bool> ComputerLabExtrudables { get; set; }

    // Tooltip Trackers
    public bool PressETooltipShown { get; set; }
    public bool PressQTooltipShown { get; set; }

    // Scene State
    public string SceneName { get; set; }

    // Constructor
    public GameState()
    {
        CurrentLevel = Level.tutorial;
        CurrentPuzzleId = -1;

        PlayerPosition3D = new Vector3(-2.38f,4.67f,24.57f);
        PlayerRotation3D = new Vector3(0f,180f,0f);
        CameraPosition3D = Vector3.zero;
        CameraRotation3D = Vector3.zero;

        Door0Unlocked = false;
        Door1Unlocked = false;
        PlayerHasUSB = false;
        USBInserted = false;

        PlayerPosition2D = new Vector3(0.13f, 2.1f, 0.0f);
        PlayerPuzzlePosition2D = new Vector3(-6.64f,-1.75f,0f);

        CurrentExtrudableSetId = -1;
        TutorialExtrudables = new List<bool> { false, false, false };
        BioLabExtrudables = new List<bool> { false, false, false, false, false };
        ComputerLabExtrudables = new List<bool> { false, false, false };

        PressETooltipShown = false;
        PressQTooltipShown = false;

        SceneName = "new3Dtut";
    }
}
