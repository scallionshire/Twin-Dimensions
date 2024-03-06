using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{   
    public GameState gameState = new GameState();
    public static GameManager instance;
    private bool sceneLoaded = true;

    // Initial puzzle states, loaded in via ScriptableObjects in the inspector
    public BlockPuzzles initTutorialPuzzle;
    public BlockPuzzles initComputerPuzzle;
    public BlockPuzzles initChemicalPuzzle;

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
        gameState.CurrTutorialPuzzle = initTutorialPuzzle;
        gameState.CurrComputerPuzzle = initComputerPuzzle;
        gameState.CurrChemicalPuzzle = initChemicalPuzzle;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {   
        Debug.Log("Scene loaded: " + scene.name);
        if (scene.name == "fbx3dtut") {
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
            
            if (gameState.DoorUnlocked) {
                GameObject.Find("Door").GetComponent<Animator>().SetBool("isOpen", true);
            }

            for (int i = 0; i < gameState.TutorialExtrudables.Count; i++) {
                if (gameState.TutorialExtrudables[i]) {
                    GameObject.Find("Extrudable" + i).GetComponent<Extrudable>().isMoving = true;
                }
            }

            if (gameState.ElevatorRunning) {
                GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
                Debug.Log("Platform moving");
                foreach (GameObject platform in platforms) {
                    platform.GetComponent<Extrudable>().isMoving = true;
                }
            }
        }

        if (scene.name == "fbx3dmain") {
            GameObject ThirdPersonCamera = GameObject.Find("Third Person Camera");
            ThirdPersonCamera.transform.position = gameState.CameraPosition3D;
            ThirdPersonCamera.transform.eulerAngles = gameState.CameraRotation3D;

            GameObject Player3D = GameObject.Find("3D Player");
            Player3D.transform.position = gameState.PlayerPosition3D;
            Player3D.transform.eulerAngles = gameState.PlayerRotation3D;


            for (int i = 0; i < gameState.BioLabExtrudables.Count; i++) {
                if (gameState.BioLabExtrudables[i]) {
                    GameObject.Find("Extrudable" + i).GetComponent<Extrudable>().isMoving = true;
                }
            }
        }

        if (scene.name == "updated2dTut") {
            PuzzleManager puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();

            puzzleManager.levelPuzzles = gameState.CurrTutorialPuzzle;

            // Note: puzzle manager will instantiate the relevant variables for us!

            // Handle extrusion states
            foreach (GameObject extrudable in GameObject.FindGameObjectsWithTag("Extrudable")) {
                Extrudable ext = extrudable.GetComponent<Extrudable>();
                if (ext != null && gameState.TutorialExtrudables[ext.extrudableId]) {
                    ext.isMoving = true;
                }
            }

            // restore player position
            GameObject Player2D = GameObject.Find("2D Player");
            Player2D.transform.position = gameState.PlayerPosition2D;
        }

        if (scene.name == "chemicalPuzzle") {
            PuzzleManager puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();

            puzzleManager.levelPuzzles = gameState.CurrChemicalPuzzle;
        }

        if (scene.name == "computerPuzzle") {
            PuzzleManager puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();

            puzzleManager.levelPuzzles = gameState.CurrComputerPuzzle;
        }

        sceneLoaded = true;
    }
    

    void Update()
    {   
        if (!sceneLoaded) {
            return;
        }

        // Player State Persistence
        if (SceneManager.GetActiveScene().name == "fbx3dtut") {
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

        if (SceneManager.GetActiveScene().name == "fbx3dmain") {
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
            GameObject Player2D = GameObject.Find("2D Player");
            if (Player2D != null) {
                gameState.PlayerPosition2D = Player2D.transform.position;
            }

            // Update positions of each block in game state
            if (gameState.CurrentPuzzleId >= 0) {
                for (int i = 0; i < gameState.CurrTutorialPuzzle.puzzles[gameState.CurrentPuzzleId].puzzleBlocks.Count; i++) {
                    if (!gameState.CurrTutorialPuzzle.puzzles[gameState.CurrentPuzzleId].puzzleBlocks[i].isSolved) {
                        PuzzleSet temp = gameState.CurrTutorialPuzzle.puzzles[gameState.CurrentPuzzleId].puzzleBlocks[i];
                        temp.blockInitPosition = GameObject.Find(temp.blockName).transform.position;
                        gameState.CurrTutorialPuzzle.puzzles[gameState.CurrentPuzzleId].puzzleBlocks[i] = temp;
                    }
                }
            }
        }

        // Scene Switch Logic
        if (Input.GetKeyDown(KeyCode.Q) && gameState.USBInserted || Input.GetKeyDown(KeyCode.M)) // M is cheat code to switch scenes
        {   
            Debug.Log("Switching scenes for level " + gameState.CurrentLevel);

            switch (gameState.CurrentLevel) {
                case Level.tutorial:
                    if (SceneManager.GetActiveScene().name == "updated2dTut")
                    {
                        sceneLoaded = false;
                        SceneManager.LoadScene("fbx3dtut");
                    }
                    else if (SceneManager.GetActiveScene().name == "fbx3dtut")
                    {
                        sceneLoaded = false;
                        SceneManager.LoadScene("updated2dTut");
                    }
                    break;
                case Level.biolab:
                    if (SceneManager.GetActiveScene().name == "2dmain" || SceneManager.GetActiveScene().name == "computerPuzzle" || SceneManager.GetActiveScene().name == "chemicalPuzzle")
                    {
                        sceneLoaded = false;
                        SceneManager.LoadScene("fbx3dmain");
                    }
                    else if (SceneManager.GetActiveScene().name == "fbx3dmain")
                    {
                        sceneLoaded = false;
                        SceneManager.LoadScene("2dmain");
                    }
                    break;
                case Level.computerlab:
                    if (SceneManager.GetActiveScene().name == "2dmain" || SceneManager.GetActiveScene().name == "computerPuzzle" || SceneManager.GetActiveScene().name == "chemicalPuzzle")
                    {
                        sceneLoaded = false;
                        SceneManager.LoadScene("fbx3dmain");
                    }
                    else if (SceneManager.GetActiveScene().name == "fbx3dmain")
                    {
                        sceneLoaded = false;
                        SceneManager.LoadScene("2dmain");
                    }
                    break;
            }
        }

        // C is a cheat code to try out the computer puzzle
        if (Input.GetKeyDown(KeyCode.C)) {
            gameState.CurrentLevel = Level.computerlab;
            sceneLoaded = false;
            gameState.CurrentPuzzleId = 0;
            SceneManager.LoadScene("computerPuzzle");
        }

        // V is a cheat code to view the monitor room
        if (Input.GetKeyDown(KeyCode.V) && SceneManager.GetActiveScene().name == "fbx3dmain") {
            GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(2.90f,11.88f,-11.48f);
        }

        // X is a cheat code to extrude everything
        if (Input.GetKeyDown(KeyCode.X)) {
            foreach (GameObject ext in GameObject.FindGameObjectsWithTag("Extrudable")) {
                ext.GetComponent<Extrudable>().Extrude();
            }
        }
    }

    public void GetUSB() {
        gameState.PlayerHasUSB = true;
        GameObject.FindGameObjectsWithTag("USB")[0].SetActive(false);
    }

    public void InsertUSB() {
        gameState.USBInserted = true;
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
                        gameState.ElevatorRunning = true;
                        break;
                    case 1:
                        gameState.DoorUnlocked = true;
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

    public void ActivateChemPuzzle() {
        gameState.ChemPuzzleUnlocked = true;
        gameState.CurrentPuzzleId = 0;
        gameState.CurrentLevel = Level.biolab;

        sceneLoaded = false;
        SceneManager.LoadScene("chemicalPuzzle");
    }

    public void ActivateComputerPuzzle() {
        gameState.ComputerPuzzleUnlocked = true;
        gameState.CurrentPuzzleId = 0;
        gameState.CurrentLevel = Level.computerlab;

        sceneLoaded = false;
        SceneManager.LoadScene("computerPuzzle");
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
    public bool DoorUnlocked { get; set; }
    public bool PlayerHasUSB { get; set; }
    public bool USBInserted { get; set; }
    public bool ElevatorRunning { get; set; }

    // Biolab
    public bool ChemPuzzleUnlocked { get; set; }
    public bool ComputerPuzzleUnlocked { get; set; }
    
    // 2D Character State
    public Vector2 PlayerPosition2D { get; set; }

    // 2D Game State
    
    // Dynamically updated instances of puzzle states
    public BlockPuzzles CurrTutorialPuzzle { get; set; }
    public BlockPuzzles CurrComputerPuzzle { get; set; }
    public BlockPuzzles CurrChemicalPuzzle { get; set; }

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

        PlayerPosition3D = new Vector3(1.95f,4.28f,24.2f);
        PlayerRotation3D = new Vector3(0f,180f,0f);
        CameraPosition3D = Vector3.zero;
        CameraRotation3D = Vector3.zero;

        DoorUnlocked = false;
        PlayerHasUSB = false;
        USBInserted = false;
        ElevatorRunning = false;

        ChemPuzzleUnlocked = false;

        PlayerPosition2D = new Vector3(0.13f, 2.1f, 0.0f);

        TutorialExtrudables = new List<bool> { false, false };
        BioLabExtrudables = new List<bool> { false, false, false, false, false };
        ComputerLabExtrudables = new List<bool> { false, false, false };

        SceneName = "fbx3dtut";
    }
}
