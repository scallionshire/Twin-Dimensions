using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cinemachine.PostFX;
using StarterAssets;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{   
    [SerializeField]
    public GameState gameState = new GameState();

     // --------------- DEBUG FUNCTIONS ---------------
    [Header("Debug-Only Variables")]
    public bool debugMode = false;
    public Level debugLevel = Level.tutorial;
    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();
    // -----------------------------------------------

    public static GameManager instance;
    private CutsceneManager cutsceneManager;
    private bool sceneLoaded = true;
    [HideInInspector]
    public bool gameStarted = false;   
    private bool cutscenePlaying = false;
    private bool menuUnloaded = false;
    private bool hasPlayedSwitchCutscene = false;

    [Header("Game Variables")]
    public bool firstSwitch = true;   
    public bool isFrozen = false;
    
    public float MusicVolume = 100f;
    public float DialogueVolume = 100f;
    public float SFXVolume = 100f;

    private FMODUnity.StudioEventEmitter eventEmitter;

    public GameObject popupMenu;
    public ExtrudableDataScriptable initTutorialExtrudables;
    public ExtrudableDataScriptable initComputerLabExtrudables;
    public PuzzleDataScriptable initTutorialPuzzle; // Initial puzzle states, loaded in via ScriptableObjects in the inspector
    public PuzzleDataScriptable initComputerPuzzle; // Initial puzzle states, loaded in via ScriptableObjects in the inspector

    [SerializeField]
    private VolumeProfile globalVolumeProfile;

    public string ActiveSceneName;

    public delegate void SwitchEventHandler();
    public static event SwitchEventHandler OnSwitch;


    public void TriggerSwitch() {
        OnSwitch?.Invoke();
    }

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

        if (debugMode) {
            Cursor.lockState = CursorLockMode.Locked;
            scenesToLoad.Add(SceneManager.LoadSceneAsync("GUI", LoadSceneMode.Additive));
            scenesToLoad.Add(SceneManager.LoadSceneAsync("mainPuzzle", LoadSceneMode.Additive));
            scenesToLoad.Add(SceneManager.LoadSceneAsync("new2dtut", LoadSceneMode.Additive));

            gameStarted = true;

            StartCoroutine(LoadAndDeactivate(scenesToLoad));
        }
    }

    void Start()
    {
        if (!debugMode) {
            cutsceneManager = GameObject.Find("CutsceneManager").GetComponent<CutsceneManager>();
        }
    
        // Doing this prevents losing reference to the popup menu
        TogglePauseMenu();
        TogglePauseMenu();
    }

    private IEnumerator<YieldInstruction> LoadAndDeactivate(List<AsyncOperation> loadOperations)
    {
        foreach (var loadOp in loadOperations)
        {   
            while (!loadOp.isDone)
            {
                yield return null; 
            }
        }
        
        DeactivateScene("new2dtut");
        DeactivateScene("mainPuzzle");
        ActiveSceneName = "new3Dtut";

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("new3Dtut"));

        instance.gameState.CurrentLevel = debugLevel;

        if (debugLevel == Level.computerlab) {
            GameObject Player3D = GameObject.Find("3D Player");
            Player3D.transform.position = new Vector3(-0.85f,-6.05f,7.03f);
            instance.gameState.PlayerPosition3D = new Vector3(-0.85f,-6.05f,7.03f);

            instance.gameState.Door0Unlocked = true;
            instance.gameState.Door1Unlocked = true;

            instance.gameState.PlayerHasUSB = true;
            instance.gameState.USBInserted = true;
        }
    }

    void Update()
    {   
        // Scene Switch Logic
        if (Input.GetButtonDown("SwitchDim") && gameState.USBInserted && (!isFrozen || firstSwitch))
        {   
            if (firstSwitch && ActiveSceneName == "new3Dtut") {
                ToggleDialogueFreeze(false);
                ToggleBokeh(false);
                GameObject.Find("TooltipCanvas").GetComponent<TooltipManager>().RemoveQTooltip();
                firstSwitch = false;
                if (cutsceneManager != null) {
                    cutsceneManager.PlayCutscene("switch");
                }
            }
            
            if (ActiveSceneName == "new3Dtut")
            {   
                SwitchToMap(gameState.CurrentExtrudableSetId, instance.gameState.CurrentLevel);
            }
            else if (ActiveSceneName == "new2dtut")
            {
                switchToScene("new3Dtut");
            }
            else if (ActiveSceneName == "mainPuzzle")
            {
                switchToScene("new3Dtut");
            }
        }

        // It's fine to toggle pause menu if we're in debug mode
        if (cutsceneManager == null && Input.GetButtonDown("Cancel") && !isFrozen) {
            TogglePauseMenu();
        }

        if (cutsceneManager != null) {
            // Don't allow pause menu to be opened during cutscenes or dialogue lol
            if (cutsceneManager.IsPlaying() == false && Input.GetButtonDown("Cancel") && !isFrozen) {
                TogglePauseMenu();
            }

            if (cutsceneManager.IsPlaying() && !cutscenePlaying && ActiveSceneName == "new3Dtut") {
                ToggleDialogueFreeze(true);
                ToggleBokeh(true);
                cutscenePlaying = true;
            } else if (cutsceneManager.IsPlaying() == false && cutscenePlaying) {
                cutscenePlaying = false;
                if (ActiveSceneName == "new3Dtut") {
                    StartCoroutine(WaitBeforeUnfreezing());
                } else {
                    ToggleDialogueFreeze(false);
                    ToggleBokeh(false);
                }
            }
        }
    }

    IEnumerator WaitBeforeUnfreezing()
    {
        yield return new WaitForSeconds(1.0f);
        ToggleDialogueFreeze(false);
        ToggleBokeh(false);
    }

    public void SwitchToPuzzle(int puzzleId, Level level) 
    {   
        switchToScene("mainPuzzle");

        if (instance.gameState.CurrentPuzzleId != puzzleId || instance.gameState.CurrentLevel != level) {
            instance.gameState.CurrentPuzzleId = puzzleId;
            instance.gameState.CurrentLevel = level;
            wipePuzzle();
            // Find PuzzleManager and load the puzzle
            PuzzleManager puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
            puzzleManager.LoadPuzzle();
        }
    }

    public void SwitchToMap(int extId, Level level) {
        switchToScene("new2dtut");
        // Find ExtrudableManager and load the map
        if (extId != instance.gameState.CurrentExtrudableSetId || level != instance.gameState.CurrentLevel) {
            instance.gameState.CurrentExtrudableSetId = extId;
            instance.gameState.CurrentLevel = level;
            wipeExtrudables();

            ExtrudableManager extrudableManager = GameObject.Find("ExtrudableManager").GetComponent<ExtrudableManager>();
            extrudableManager.LoadMap();
        }
    }

    private void UpdateInventoryUI() {
        InventorySystem inventorySystem = FindObjectOfType<InventorySystem>();
        if (inventorySystem != null) {
            Debug.Log("Updating inventory UI");
            inventorySystem.UpdateInventoryUI();
        }
    }

    public void GetUSB() {
        instance.gameState.PlayerHasUSB = true;
        Debug.Log("Adding USB to inventory");
        UpdateInventoryUI();
        GameObject.FindGameObjectWithTag("USB").SetActive(false);
    }

    public void InsertUSB() {
        if (instance.gameState.PlayerHasUSB) {
            instance.gameState.USBInserted = true;
            // gameState.USBInserted = true;
            Debug.Log("Game state, USB inserted: " + instance.gameState.USBInserted);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX3D/USBInsert");
        } else {
            Debug.Log("Player does not have USB");
        }
    }

    public void PickUpBattery(GameObject battery) {
        instance.gameState.BatteriesCollected++;
        Debug.Log($"Battery collected: {instance.gameState.BatteriesCollected}/{instance.gameState.TotalBatteries}");

        if (instance.gameState.BatteriesCollected == instance.gameState.TotalBatteries)
        {
            Debug.Log("All batteries collected!");
        }

        UpdateInventoryUI();
        Destroy(battery); 
        RotateDetectionZone rotatingMonitor = GameObject.Find("MonitorCylinder").GetComponent<RotateDetectionZone>();
        rotatingMonitor.IncreaseRotationSpeed();
    }

    public void UseBattery() {
        if (gameState.BatteriesCollected > 0) {
            gameState.BatteriesCollected--;
            Debug.Log($"Battery used: Now {gameState.BatteriesCollected}/{gameState.TotalBatteries} remaining.");
            UpdateInventoryUI(); 
        }
        else {
            Debug.Log("No batteries to use.");
        }
    }

    public void UpdateExtrudables(int extrudableId) {
        instance.gameState.Extrudables[extrudableId] = true;
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
                switch (puzzleId) {
                    case 0:
                        instance.gameState.BlueOverlayOn = true;
                        break;
                    case 1:
                        instance.gameState.PinkOverlayOn = true;
                        break;
                }
                break;
        }

        switchToScene("new3Dtut");
    }

    // TODO: clean this up
    public void SolvePuzzleBlock(Level level, int blockId) {
        switch (level) {
            case Level.tutorial:
                break;
            case Level.biolab:
                break;
            case Level.computerlab:
                switch (blockId) {
                    case 0:
                        instance.gameState.BlueGroup0On = true;
                        break;
                    case 1:
                        instance.gameState.BlueGroup1On = true;
                        break;
                    case 2:
                        instance.gameState.BlueGroup2On = true;
                        break;
                }
                break;
        }
    }

    public void SetCurrentPuzzle(int puzzleId) {
        instance.gameState.CurrentPuzzleId = puzzleId;
    }

    public void TogglePauseMenu()
    {
        if (popupMenu.activeSelf)
        {
            popupMenu.SetActive(false);
            Time.timeScale = 1f;
            if (ActiveSceneName == "new3Dtut" || ActiveSceneName == "mainPuzzle" || ActiveSceneName == "new2dtut") {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            popupMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void DeactivateScene(string sceneName)
    {   
        DialogueManager dialogueManager = gameObject.GetComponent<DialogueManager>();
        dialogueManager.EndDialogue();

        Scene scene = SceneManager.GetSceneByName(sceneName);
        TriggerSwitch(); // Trigger switch that removes player input
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            obj.SetActive(false);
        }
    }

    public void ActivateScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        ActiveSceneName = sceneName;
        if (scene.isLoaded)
        {   
            SceneManager.SetActiveScene(scene);
            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (GameObject obj in rootObjects)
            {
                obj.SetActive(true);
            }
        }
    }

    public void OnSceneSwitch()
    {
        // Utility function to handle effects that need to be checked when switching scenes
        if (instance.gameState.Door0Unlocked) {
            GameObject Door0 = GameObject.Find("Door0");
            if (Door0 != null && Door0.GetComponent<Animator>().GetBool("isOpen") == false) {
                Door0.GetComponent<Animator>().SetBool("isOpen", true);
            }
        }
        
        if (instance.gameState.Door1Unlocked) {
            GameObject Door1 = GameObject.Find("Door1");
            if (Door1 != null && Door1.GetComponent<Animator>().GetBool("isOpen") == false) {
                Door1.GetComponent<Animator>().SetBool("isOpen", true);
            }
        }

        if (instance.gameState.Extrudables[2]) {
            instance.gameState.CurrentLevel = Level.computerlab;
        }

        // TODO: Add variables to check if extrudables are already extruded
        for (int i = 0; i < instance.gameState.Extrudables.Count; i++) {
            if (instance.gameState.Extrudables[i]) {
                if (GameObject.Find("Extrudable" + i) != null) {
                    GameObject.Find("Extrudable" + i).GetComponent<Extrudable>().isMoving = true;
                }
            }
        }

        if (ActiveSceneName == "new2dtut" || ActiveSceneName == "mainPuzzle") {
            GameObject.Find("Light 2D").GetComponent<Light2D>().enabled = true;
            GameObject.Find("TooltipCanvas").GetComponent<TooltipManager>().ToggleClickTooltip(false);
        } else {
            GameObject.Find("Light 2D").GetComponent<Light2D>().enabled = false;
        }

        // Lighting up the blue screens in the computer lab
        if (instance.gameState.BlueGroup0On) {
            GameObject blueGroup0 = GameObject.Find("BlueGroup0");
            if (blueGroup0 != null) {
                blueGroup0.GetComponent<ToggleScreen>().TurnOnScreen();
            }
        }
        if (instance.gameState.BlueGroup1On) {
            GameObject blueGroup1 = GameObject.Find("BlueGroup1");
            if (blueGroup1 != null) {
                blueGroup1.GetComponent<ToggleScreen>().TurnOnScreen();
            }
        }
        if (instance.gameState.BlueGroup2On) {
            GameObject blueGroup2 = GameObject.Find("BlueGroup2");
            if (blueGroup2 != null) {
                blueGroup2.GetComponent<ToggleScreen>().TurnOnScreen();
            }
        }

        // Lighting up the blue overlay on the final TV
        if (instance.gameState.BlueOverlayOn) {
            GameObject blueOverlay = GameObject.Find("BlueOverlay");
            if (blueOverlay != null) {
                GameObject.Find("BlueOverlay").GetComponent<SpriteRenderer>().enabled = true;
            }
        }

        if (instance.gameState.BlueOverlayOn && instance.gameState.PinkOverlayOn) {
            // Play final cutscene; priority over group cutscenes
            GameObject ComputerLabDoneTrigger = GameObject.Find("ComputerLabDoneTrigger");
            if (ComputerLabDoneTrigger != null) {
                ComputerLabDoneTrigger.GetComponent<Interactable>().Interact();
            }

            // Open the door
            GameObject Door2 = GameObject.Find("Door2");
            if (Door2 != null && Door2.GetComponent<Animator>().GetBool("isOpen") == false) {
                Door2.GetComponent<Animator>().SetBool("isOpen", true);
            }
        } else if (instance.gameState.BlueOverlayOn || instance.gameState.PinkOverlayOn) {
            // Play overlay cutscene
            GameObject LookAtOverlayTrigger = GameObject.Find("LookAtOverlayTrigger");
            if (LookAtOverlayTrigger != null) {
                LookAtOverlayTrigger.GetComponent<Interactable>().Interact();
            }
        } else if (instance.gameState.BlueGroup0On || instance.gameState.BlueGroup1On || instance.gameState.BlueGroup2On) {
            // Play group cutscene
            GameObject BlueWallDoneTrigger = GameObject.Find("BlueWallDoneTrigger");
            if (BlueWallDoneTrigger != null) {
                BlueWallDoneTrigger.GetComponent<Interactable>().Interact();
            }
        }
    }

    private void wipeExtrudables()
    {
        // Delete all gameobjects with tag: Extrudable
        GameObject[] extrudables = GameObject.FindGameObjectsWithTag("Extrudable");
        GameObject[] dialogueTriggers = GameObject.FindGameObjectsWithTag("DialogueTrigger");

        foreach (GameObject extrudable in extrudables)
        {
            if (extrudable.GetComponent<SpriteRenderer>() != null) {
                // Only destroy if 2D
                Destroy(extrudable);
            }
        }

        foreach (GameObject dialogueTrigger in dialogueTriggers)
        {
            Destroy(dialogueTrigger);
        }
    }

    private void wipePuzzle()
    {
        // Delete all gameobjects with tag: Block, BlockTrigger, and Connector
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        GameObject[] blockTriggers = GameObject.FindGameObjectsWithTag("BlockTrigger");
        GameObject[] connectors = GameObject.FindGameObjectsWithTag("Connector");
        GameObject[] dialogueTriggers = GameObject.FindGameObjectsWithTag("DialogueTrigger");

        foreach (GameObject block in blocks)
        {
            Destroy(block);
        }

        foreach (GameObject blockTrigger in blockTriggers)
        {
            Destroy(blockTrigger);
        }

        foreach (GameObject connector in connectors)
        {
            Destroy(connector);
        }

        foreach (GameObject dialogueTrigger in dialogueTriggers)
        {
            Destroy(dialogueTrigger);
        }
    }

    public void switchToScene(string sceneName)
    {
        if (eventEmitter == null) {
            Debug.Log("Event emitter not found");
            eventEmitter = GameObject.Find("Main Camera").GetComponent<FMODUnity.StudioEventEmitter>();

            // Ensure cursor is focused when you first start the game
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (eventEmitter.EventInstance.isValid()) {
            if (sceneName == "new3Dtut") {
                eventEmitter.EventInstance.setParameterByName("CurrentDimension", 0);
            } else {
                eventEmitter.EventInstance.setParameterByName("CurrentDimension", 1);
            }
        }
        
        DeactivateScene(ActiveSceneName);
        ActivateScene(sceneName);
        ActiveSceneName = sceneName;
        OnSceneSwitch();
    }

    public void ToggleCutsceneFreeze(bool freeze)
    {
        isFrozen = freeze;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        // Disable player movement
        ThirdPersonController playerController = player.GetComponent<ThirdPersonController>();
        playerController.enabled = !freeze;

        // Disable player interaction
        Interaction playerInteraction = player.GetComponent<Interaction>();
        playerInteraction.enabled = !freeze;
    }

    public void ToggleDialogueFreeze(bool freeze)
    {
        isFrozen = freeze;
        
        if (ActiveSceneName == "new3Dtut") {
            Debug.Log("Freezing player in 3D scene: " + freeze);
            // Disable camera
            GameObject playerCameraGO = GameObject.Find("Third Person Camera");
            if (playerCameraGO != null) {
                CinemachineFreeLook playerCamera = playerCameraGO.GetComponent<CinemachineFreeLook>();
                if (freeze) {
                    playerCamera.m_YAxis.m_MaxSpeed = 0;
                    playerCamera.m_XAxis.m_MaxSpeed = 0;
                } else {
                    playerCamera.m_YAxis.m_MaxSpeed = 4;
                    playerCamera.m_XAxis.m_MaxSpeed = 300;
                }
            }

            // Disable player movement
            ThirdPersonController player = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
            player.enabled = !freeze;

            // Disable player interaction
            Interaction playerInteraction = player.GetComponent<Interaction>();
            playerInteraction.enabled = !freeze;
        }

        if (ActiveSceneName == "new2dtut" || ActiveSceneName == "mainPuzzle") {
            // Disable player movement
            Player2DMovement player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player2DMovement>();
            player.enabled = !freeze;

            // Dim light if frozen
            Light2D sceneLight = GameObject.Find("Light 2D").GetComponent<Light2D>();

            sceneLight.enabled = true;

            if (freeze) {
                sceneLight.intensity = 0.5f;
            } else {
                sceneLight.intensity = 1.0f;
            }
        }
    }

    public void ToggleBokeh(bool enableBokeh)
    {
        if (enableBokeh) {
            StartCoroutine(EaseBokeh(0.1f));
        } else {
            StartCoroutine(EaseBokeh(10.0f));
        }
    }

    IEnumerator EaseBokeh(float target)
    {
        globalVolumeProfile.TryGet(out DepthOfField depthOfField);
        float start = depthOfField.focusDistance.value;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / 0.5f;
            depthOfField.focusDistance.value = Mathf.Lerp(start, target, t);
            yield return null;
        }
    }
}

[System.Serializable]
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

    // ComputerLab
    public int BatteriesCollected { get; set; }
    public int TotalBatteries { get; } = 5;
    
    // 2D Character State
    public Vector2 PlayerPosition2D { get; set; }
    public Vector2 PlayerPuzzlePosition2D { get; set; }

    // Lists of bools indicating whether or not the corresponding extrudable block in that level has been extruded
    public int CurrentExtrudableSetId { get; set; }

    public List<bool> Extrudables { get; set; }

    public bool BlueOverlayOn { get; set; }
    public bool PinkOverlayOn { get; set; }

    public bool BlueGroup0On { get; set; }
    public bool BlueGroup1On { get; set; }
    public bool BlueGroup2On { get; set; }

    // Scene State
    public string SceneName { get; set; }

    // Constructor
    public GameState()
    {
        CurrentLevel = Level.tutorial;
        CurrentPuzzleId = -1;

        PlayerPosition3D = new Vector3(-4.05f,3.541f,28.56f);
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
        Extrudables = new List<bool> { false, false, false, false, false, false };

        BlueOverlayOn = false;
        PinkOverlayOn = false;

        BlueGroup0On = false;
        BlueGroup1On = false;
        BlueGroup2On = false;

        SceneName = "new3Dtut";
    }
}
