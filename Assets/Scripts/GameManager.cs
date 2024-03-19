using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cinemachine.PostFX;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{   
    public GameState gameState = new GameState();
    public GameObject popupMenu;
    public static GameManager instance;
    private CutsceneManager cutsceneManager;

    private bool sceneLoaded = true;
    public bool gameStarted = false;   
    private bool menuUnloaded = false;
    private bool cutscenePlaying = false;
    private bool hasPlayedSwitchCutscene = false;

    public float MusicVolume = 100f;
    public float DialogueVolume = 100f;
    public float SFXVolume = 100f;

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
    }

    void Start()
    {
        cutsceneManager = GameObject.Find("CutsceneManager").GetComponent<CutsceneManager>();

        // Doing this prevents losing reference to the popup menu
        TogglePauseMenu();
        TogglePauseMenu();
    }

    void Update()
    {   
        if (SceneManager.GetActiveScene().name == "StartMenu") {
            Cursor.lockState = CursorLockMode.None;
        }
        
        // Scene Switch Logic
        if (Input.GetKeyDown(KeyCode.Q) && gameState.USBInserted) // M is cheat code to switch scenes
        {   
            Debug.Log("Switching scenes for level " + gameState.CurrentLevel);
            if (ActiveSceneName == "new3Dtut")
            {   
                if (hasPlayedSwitchCutscene == false)
                {
                    cutsceneManager.PlayCutscene("switch");
                    hasPlayedSwitchCutscene = true;
                }
                SwitchToMap(gameState.CurrentExtrudableSetId, gameState.CurrentLevel);
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

        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePauseMenu();
        }
    }

    public void SwitchToPuzzle(int puzzleId, Level level) 
    {   
        Debug.Log("Switching to puzzle: " + puzzleId);
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
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX3D/Pickup");
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
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX3D/Pickup");
        Destroy(battery); 
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
                        instance.gameState.RedOverlayOn = true;
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
            Cursor.lockState = CursorLockMode.Locked;
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

    private void OnSceneSwitch()
    {
        // Utility function to handle effects that need to be checked when switching scenes
        if (instance.gameState.Door0Unlocked) {
            if (GameObject.Find("Door0") != null) {
                GameObject.Find("Door0").GetComponent<Animator>().SetBool("isOpen", true);
            }
        }
        
        if (instance.gameState.Door1Unlocked) {
            if (GameObject.Find("Door1") != null) {
                GameObject.Find("Door1").GetComponent<Animator>().SetBool("isOpen", true);
            }
        }

        for (int i = 0; i < instance.gameState.Extrudables.Count; i++) {
            if (instance.gameState.Extrudables[i]) {
                if (GameObject.Find("Extrudable" + i) != null) {
                    GameObject.Find("Extrudable" + i).GetComponent<Extrudable>().isMoving = true;
                }
            }
        }

        if (ActiveSceneName == "new2dtut" || ActiveSceneName == "mainPuzzle") {
            GameObject.Find("Light 2D").GetComponent<Light2D>().enabled = true;
        } else {
            GameObject.Find("Light 2D").GetComponent<Light2D>().enabled = false;
        }

        if (instance.gameState.BlueOverlayOn) {
            GameObject.Find("BlueOverlay").GetComponent<SpriteRenderer>().enabled = true;
        }
        if (instance.gameState.RedOverlayOn) {
            GameObject.Find("RedOverlay").GetComponent<SpriteRenderer>().enabled = true;
        }

        if (instance.gameState.BlueOverlayOn && instance.gameState.RedOverlayOn) {
            if (GameObject.Find("Door2") != null) {
                GameObject.Find("Door2").GetComponent<Animator>().SetBool("isOpen", true);
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
        DeactivateScene(ActiveSceneName);
        ActivateScene(sceneName);
        ActiveSceneName = sceneName;
        OnSceneSwitch();
    }

    public void ToggleDialogueFreeze(bool freeze)
    {
        if (ActiveSceneName == "new3Dtut") {
            CinemachineFreeLook playerCamera = GameObject.Find("Third Person Camera").GetComponent<CinemachineFreeLook>();
            if (freeze) {
                playerCamera.m_YAxis.m_MaxSpeed = 0;
                playerCamera.m_XAxis.m_MaxSpeed = 0;
            } else {
                playerCamera.m_YAxis.m_MaxSpeed = 4;
                playerCamera.m_XAxis.m_MaxSpeed = 350;
            }

            ThirdPersonController player = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
            player.enabled = !freeze;

            // TriggerSwitch(); // freeze player input
            ToggleBokeh(freeze);
        }

        if (ActiveSceneName == "new2dtut" || ActiveSceneName == "mainPuzzle") {
            Player2DMovement player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player2DMovement>();
            player.enabled = !freeze;

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
    public bool RedOverlayOn { get; set; }

    // Scene State
    public string SceneName { get; set; }

    // Constructor
    public GameState()
    {
        CurrentLevel = Level.tutorial;
        CurrentPuzzleId = -1;

        PlayerPosition3D = new Vector3(-4.05f,3.25f,28.57f);
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
        RedOverlayOn = false;

        SceneName = "new3Dtut";
    }
}
