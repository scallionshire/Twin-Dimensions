using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cinemachine.PostFX;
using FMOD.Studio;
using StarterAssets;
using Unity.VisualScripting;
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
    public GameStateCondition debugCondition;
    // -----------------------------------------------

    public static GameManager instance;
    private CutsceneManager cutsceneManager;
    [HideInInspector]
    public bool gameStarted = false;   
    private bool cutscenePlaying = false;

    [Header("Game Variables")]
    public bool firstSwitch = true;   
    [HideInInspector]
    public bool isFrozen = false;
    
    public float MusicVolume = 1f;
    public float DialogueVolume = 1f;
    public float SFXVolume = 1f;

    private FMODUnity.StudioEventEmitter eventEmitter;

    public GameObject settingsMenu;
    public TopDownDataScriptable[] roomData;
    public ExtrudableDataScriptable tutorialExtrudables;
    public ExtrudableDataScriptable computerLabExtrudables;
    public PuzzleDataScriptable tutorialPuzzle; // Initial puzzle states, loaded in via ScriptableObjects in the inspector
    public PuzzleDataScriptable computerPuzzle; // Initial puzzle states, loaded in via ScriptableObjects in the inspector

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

            if (SceneManager.GetActiveScene().name == "new3Dtut") {
                scenesToLoad.Add(SceneManager.LoadSceneAsync("new2dtut", LoadSceneMode.Additive));
                scenesToLoad.Add(SceneManager.LoadSceneAsync("mainPuzzle", LoadSceneMode.Additive));
            } else if (SceneManager.GetActiveScene().name == "new2dtut") {
                scenesToLoad.Add(SceneManager.LoadSceneAsync("new3Dtut", LoadSceneMode.Additive));
                scenesToLoad.Add(SceneManager.LoadSceneAsync("mainPuzzle", LoadSceneMode.Additive));
            } else if (SceneManager.GetActiveScene().name == "mainPuzzle") {
                scenesToLoad.Add(SceneManager.LoadSceneAsync("new2dtut", LoadSceneMode.Additive));
                scenesToLoad.Add(SceneManager.LoadSceneAsync("new3Dtut", LoadSceneMode.Additive));
            }

            gameStarted = true;

            StartCoroutine(LoadAndDeactivate(scenesToLoad));
        }
    }

    void Start()
    {
        if (!debugMode) {
            cutsceneManager = GameObject.Find("CutsceneManager").GetComponent<CutsceneManager>();
            eventEmitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
        }

        if (GameObject.Find("SettingsMenu") != null) {
            settingsMenu = GameObject.Find("SettingsMenu");
            settingsMenu.SetActive(false);
        }

        MusicVolume = 1f;
        DialogueVolume = 1f;
        SFXVolume = 1f;
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
        
        if (SceneManager.GetActiveScene().name == "new3Dtut") {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("new3Dtut"));
            DeactivateScene("new2dtut");
            DeactivateScene("mainPuzzle");
            ActiveSceneName = "new3Dtut";
        } else if (SceneManager.GetActiveScene().name == "new2dtut") {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("new2dtut"));
            DeactivateScene("new3Dtut");
            DeactivateScene("mainPuzzle");
            ActiveSceneName = "new2dtut";
        } else if (SceneManager.GetActiveScene().name == "mainPuzzle") {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("mainPuzzle"));
            DeactivateScene("new3Dtut");
            DeactivateScene("new2dtut");
            ActiveSceneName = "mainPuzzle";
        }

        instance.gameState.CurrentLevel = debugLevel;

        // Skip to computer lab level
        if (debugLevel == Level.computerlab) {
            GameObject Player3D = GameObject.Find("3D Player");
            Player3D.transform.position = new Vector3(-0.85f,-4.05f,7.03f);
            instance.gameState.PlayerPosition3D = new Vector3(-0.85f,-6.05f,7.03f);

            instance.gameState.Door0Unlocked = true;
            instance.gameState.Door1Unlocked = true;

            instance.gameState.PlayerHasUSB = true;
            instance.gameState.USBInserted = true;

            instance.firstSwitch = false;
        }

        // Unlock current conditions
        switch (debugCondition)
        {
            case GameStateCondition.hasUSB:
                instance.gameState.PlayerHasUSB = true;
                break;
            case GameStateCondition.insertedUSB:
                instance.gameState.PlayerHasUSB = true;
                instance.gameState.USBInserted = true;
                break;
            case GameStateCondition.hasBattery:
                instance.gameState.PlayerHasUSB = true;
                instance.gameState.USBInserted = true;
                instance.gameState.BatteriesCollected = 5;
                break;
        }
    }

    void Update()
    {   
        // Only allow player to switch dimensions if they have inserted the USB, and a cutscene isn't playing
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
                SwitchToMap(instance.gameState.CurrentLevel, true);
            } else {
                if (ActiveSceneName == "new3Dtut")
                {   
                    Debug.Log("Switching to map!");
                    SwitchToMap(instance.gameState.CurrentLevel);
                }
                else if (ActiveSceneName == "new2dtut")
                {
                    switchToScene("new3Dtut");
                }
                else if (ActiveSceneName == "mainPuzzle")
                {
                    switchToScene("new2dtut");
                }
            }
        }

        // It's fine to toggle pause menu if we're in debug mode
        if (cutsceneManager == null && Input.GetButtonDown("Cancel") && !isFrozen) {
            TogglePauseMenu();
        } else if (cutsceneManager != null) {
            // Don't allow pause menu to be opened during cutscenes or dialogue lol
            if (cutsceneManager.IsPlaying() == false && Input.GetButtonDown("Cancel") && !isFrozen) {
                TogglePauseMenu();
            }

            if (cutsceneManager.IsPlaying() && !cutscenePlaying && ActiveSceneName == "new3Dtut") {
                PauseMainMusic(true);
                ToggleDialogueFreeze(true);
                ToggleBokeh(true);
                cutscenePlaying = true;
                if (eventEmitter == null) {
                    eventEmitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
                }
                if (eventEmitter.EventInstance.isValid())
                {
                    eventEmitter.EventInstance.setPaused(true);
                }
            } else if (cutsceneManager.IsPlaying() == false && cutscenePlaying) {
                PauseMainMusic(false);
                cutscenePlaying = false;
                if (ActiveSceneName == "new3Dtut") {
                    if (eventEmitter == null) {
                        eventEmitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
                    }
                    if (eventEmitter.EventInstance.isValid()) {
                        eventEmitter.EventInstance.setPaused(false);
                    }
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

    public void SwitchToPuzzle(int puzzleId, Level level, bool newMap = false) 
    {   
        switchToScene("mainPuzzle");

        if (newMap || instance.gameState.CurrentPuzzleId != puzzleId || instance.gameState.CurrentLevel != level) {
            instance.gameState.CurrentPuzzleId = puzzleId;
            instance.gameState.CurrentLevel = level;
            // Find PuzzleManager and load the puzzle
            PuzzleManager puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
            puzzleManager.wipePuzzle();
            puzzleManager.LoadPuzzle();
        }
    }

    public void SwitchToMap(Level level, bool newMap = false) {
        switchToScene("new2dtut");

        if (newMap || instance.gameState.RoomChanged) {
            instance.gameState.CurrentLevel = level;
            
            RoomManager roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
            roomManager.WipeScene();
            roomManager.LoadCurrentScene();

            instance.gameState.RoomChanged = false;
        }
    }

    // --------------- GAME STATE FUNCTIONS ---------------
    private void UpdateInventoryUI() {
        InventorySystem inventorySystem = FindObjectOfType<InventorySystem>();
        if (inventorySystem != null) {
            inventorySystem.UpdateInventoryUI();
        }
    }

    public void GetUSB() {
        instance.gameState.PlayerHasUSB = true;
        UpdateInventoryUI();
        GameObject.FindGameObjectWithTag("USB").SetActive(false);
    }

    public void InsertUSB() {
        if (instance.gameState.PlayerHasUSB) {
            instance.gameState.USBInserted = true;
            // gameState.USBInserted = true;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX3D/USBInsert");
        }
    }

    public void PickUpBattery(GameObject battery) {
        instance.gameState.BatteriesCollected++;

        UpdateInventoryUI();
        Destroy(battery); 
        RotateDetectionZone rotatingMonitor = GameObject.Find("MonitorCylinder").GetComponent<RotateDetectionZone>();
        rotatingMonitor.IncreaseRotationSpeed();
    }

    public void UseBattery() {
        if (gameState.BatteriesCollected > 0) {
            gameState.BatteriesCollected--;
            UpdateInventoryUI(); 
        }
    }

    public void UpdateExtrudables(int extrudableId) {
        instance.gameState.Extrudables[extrudableId] = true;
    }

    public void UpdateActivatedPanel(int puzzleId, Level level) {
        switch (level) {
            case (Level.tutorial):
                instance.gameState.TutorialLevelPorts[puzzleId] = true;
                break;
            case (Level.computerlab):
                instance.gameState.ComputerLabLevelPorts[puzzleId] = true;
                break;
        }
    }

    public void SolvePuzzleBlock(Level level, int puzzleId, int blockId) {
        switch (level) {
            case Level.tutorial:
                switch (puzzleId) {
                    case 2:
                        switch (blockId) {
                            case 0:
                                instance.gameState.Extrudables[0] = true;
                                break;
                            case 1:
                                instance.gameState.Extrudables[1] = true;
                                break;
                        }
                        break;
                    case 4:
                        switch (blockId) {
                            case 0:
                                instance.gameState.Extrudables[3] = true;
                                break;
                            case 1:
                                instance.gameState.Extrudables[4] = true;
                                break;
                        }
                        break;
                }
                break;
            case Level.computerlab:
                switch (puzzleId) {
                    case 0:
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
                    case 2:
                        instance.gameState.Extrudables[0] = true;
                        instance.gameState.Extrudables[1] = true;
                        break;
                    case 3:
                        instance.gameState.Extrudables[2] = true;
                        break;
                    case 4:
                        instance.gameState.Extrudables[3] = true;
                        instance.gameState.Extrudables[4] = true;
                        break;
                    case 5:
                        instance.gameState.Extrudables[5] = true;
                        break;
                }
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

        if (!(level == Level.computerlab && puzzleId == 1)) {
            switchToScene("new2dtut");
        }
    }
    
    public void UpdateMusicVolume(float volume)
    {
        MusicVolume = volume;
        if (eventEmitter == null || !eventEmitter.EventInstance.isValid()) {
            var eventEmitters = GameObject.FindObjectsOfType<FMODUnity.StudioEventEmitter>();
            eventEmitter = eventEmitters[0];
        }
        if (eventEmitter.EventInstance.isValid())
        {
            eventEmitter.EventInstance.setVolume(volume);
        }
    }

    public void UpdateSFXVolume(float volume)
    {
        SFXVolume = volume;
        FMOD.Studio.Bus sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        sfxBus.setVolume(volume);
    }

    public void PauseMainMusic(bool pause)
    {
        if (eventEmitter == null || !eventEmitter.EventInstance.isValid()) {
            Debug.Log("eventEmitter was null or not valid");
            eventEmitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
        }
        if (eventEmitter != null && eventEmitter.EventInstance.isValid())
        {
            Debug.Log("Setting isPaused");
            if (pause)
            {
                eventEmitter.EventInstance.setParameterByName("isPaused", 1);
            }
            else
            {
                eventEmitter.EventInstance.setParameterByName("isPaused", 0);
            }
        }
    }

    public void SetCurrentLevel(int level) {
        // Event invoker can only handle ints as parameters, so we need to do this
        switch (level) {
            case 0:
                instance.gameState.CurrentLevel = Level.tutorial;
                break;
            case 1:
                instance.gameState.CurrentLevel = Level.computerlab;
                break;
        }

        // New level, so map should be blank right now
        instance.gameState.CurrentExtrudableSetId = 0;
    }

    public void SetCurrentRoom(int room) {
        // Debug.Log("Setting room to " + room);
        if (instance.gameState.CurrentRoom != room) {
            instance.gameState.RoomChanged = true;
        }
        instance.gameState.CurrentRoom = room;
    }

    // --------------- SCENE MANAGEMENT FUNCTIONS ---------------
    public void TogglePauseMenu()
    {
        if (settingsMenu == null) {
            settingsMenu = GameObject.Find("SettingsMenu");
            settingsMenu.SetActive(false);
        }

        if (settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
            Time.timeScale = 1f;
            if (ActiveSceneName == "new3Dtut" || ActiveSceneName == "mainPuzzle" || ActiveSceneName == "new2dtut") {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            settingsMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void DeactivateScene(string sceneName)
    {   
        DialogueManager dialogueManager = gameObject.GetComponent<DialogueManager>();
        if (dialogueManager.dialogueActive) {
            dialogueManager.EndDialogue();
        }

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

    // Utility function to handle effects that need to be checked when switching scenes
    public void OnSceneSwitch()
    {
        // Unlock doors if they are unlocked, and use flags to make sure animations are only played once
        if (instance.gameState.Door0Unlocked) {
            GameObject Door0 = GameObject.Find("Door0");
            if (Door0 != null) {
                // check if we're in 3d or 2d scene and play animation once
                if (ActiveSceneName == "new3Dtut") {
                    if (instance.gameState.Door0AnimPlayed3D) {
                        Destroy(Door0);
                    } else {
                        Door0.GetComponent<Animator>().SetBool("isOpen", true);
                        instance.gameState.Door0AnimPlayed3D = true;
                    }
                } else {
                    if (instance.gameState.Door0AnimPlayed2D) {
                        Destroy(Door0);
                    } else {
                        Door0.GetComponent<Animator>().SetBool("isOpen", true);
                        instance.gameState.Door0AnimPlayed2D = true;
                    }
                }
            }
        }
        
        if (instance.gameState.Door1Unlocked) {
            GameObject Door1 = GameObject.Find("Door1");
            if (Door1 != null) {
                if (ActiveSceneName == "new3Dtut") {
                    if (instance.gameState.Door1AnimPlayed3D) {
                        Destroy(Door1);
                    } else {
                        Door1.GetComponent<Animator>().SetBool("isOpen", true);
                        instance.gameState.Door1AnimPlayed3D = true;
                    }
                } else {
                    if (instance.gameState.Door1AnimPlayed2D) {
                        Destroy(Door1);
                    } else {
                        Door1.GetComponent<Animator>().SetBool("isOpen", true);
                        instance.gameState.Door1AnimPlayed2D = true;
                    }
                }
            }
        }

        for (int i = 0; i < instance.gameState.Extrudables.Count; i++) {
            if (instance.gameState.Extrudables[i]) {
                GameObject exti = GameObject.Find("Extrudable" + i);
                if (exti != null) {
                    Extrudable extrudable = exti.GetComponent<Extrudable>();
                    if (extrudable != null) {
                        extrudable.isMoving = true;
                    } else {
                        if (instance.gameState.ExtrudablesAnimPlayed2D[i]) {
                            Debug.Log("Extrudable animation already played");
                            exti.GetComponent<Animator>().enabled = false;
                            GameObject RoomManager = GameObject.Find("RoomManager");
                            RoomManager?.GetComponent<RoomManager>().SwapExtrudableSprite(exti.name);
                        } else {
                            if (exti.GetComponent<Animator>() != null) {
                                exti.GetComponent<Animator>().SetBool("Extruded", true);
                                instance.gameState.ExtrudablesAnimPlayed2D[i] = true;
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < instance.gameState.TutorialLevelPorts.Count; i++) {
            if (instance.gameState.TutorialLevelPorts[i]) {
                GameObject RoomManager = GameObject.Find("RoomManager");
                RoomManager?.GetComponent<RoomManager>().ActivatePanel(i);
            }
        }

        for (int i = 0; i < instance.gameState.ComputerLabLevelPorts.Count; i++) {
            if (instance.gameState.ComputerLabLevelPorts[i]) {
                GameObject RoomManager = GameObject.Find("RoomManager");
                RoomManager?.GetComponent<RoomManager>().ActivatePanel(i);
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

    public void switchToScene(string sceneName)
    {
        if (eventEmitter == null) {
            eventEmitter = Camera.main.GetComponent<FMODUnity.StudioEventEmitter>();
            Debug.Log("Found event emitter: " + eventEmitter);
            // Ensure cursor is focused when you first start the game
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (eventEmitter != null && eventEmitter.EventInstance.isValid()) {
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

    public void ToggleDialogueFreeze(bool freeze)
    {
        isFrozen = freeze;
        
        if (ActiveSceneName == "new3Dtut") {
            // Disable camera
            GameObject playerCameraGO = GameObject.Find("Third Person Camera");
            if (playerCameraGO != null) {
                CinemachineFreeLook playerCamera = playerCameraGO.GetComponent<CinemachineFreeLook>();
                if (freeze) {
                    playerCamera.m_YAxis.m_MaxSpeed = 0;
                    playerCamera.m_XAxis.m_MaxSpeed = 0;
                } else {
                    playerCamera.m_YAxis.m_MaxSpeed = 4;
                    playerCamera.m_XAxis.m_MaxSpeed = 200;
                }
            }

            // Disable player movement
            ThirdPersonController player = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
            player.enabled = !freeze;

            Animator playerAnimator = player.transform.Find("Mesh").gameObject.GetComponent<Animator>();
            playerAnimator.SetFloat("Speed", 0);

            // Disable player interaction
            Interaction playerInteraction = player.GetComponent<Interaction>();
            playerInteraction.enabled = !freeze;
        }

        if (ActiveSceneName == "new2dtut" || ActiveSceneName == "mainPuzzle") {
            // Disable player movement
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Player2DMovement playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<Player2DMovement>();
            playerMovement.enabled = !freeze;
            Interaction2D playerInteraction = player.GetComponent<Interaction2D>();
            playerInteraction.enabled = !freeze;

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
    public int CurrentRoom { get; set; } // current room that 02 is in
    public bool RoomChanged { get; set; } // flag to indicate if the room has changed

    // 3D Character State
    public Vector3 PlayerPosition3D { get; set; }
    public Vector3 PlayerRotation3D { get; set; }
    public Vector3 CameraPosition3D { get; set; }
    public Vector3 CameraRotation3D { get; set; }

    // 3D Game State
    // Tutorial
    public bool Door0Unlocked { get; set; }
    public bool Door0AnimPlayed3D { get; set; }
    public bool Door0AnimPlayed2D { get; set; }
    public bool Door1Unlocked { get; set; }
    public bool Door1AnimPlayed3D { get; set; }
    public bool Door1AnimPlayed2D { get; set; }
    public bool PlayerHasUSB { get; set; }
    public bool USBInserted { get; set; }

    // ComputerLab
    public int BatteriesCollected { get; set; }
    public int TotalBatteries { get; } = 5;
    
    // 2D Character State
    public Vector2 PlayerPosition2D { get; set; }
    public Vector2 PlayerPuzzlePosition2D { get; set; }

    // Lists of bools indicating whether or not the corresponding extrudable block in that level has been extruded
    public int CurrentExtrudableSetId { get; set; }

    public List<bool> Extrudables { get; set; }
    public List<bool> ExtrudablesAnimPlayed2D { get; set; }

    public List<bool> TutorialLevelPorts { get; set; }
    public List<bool> ComputerLabLevelPorts { get; set; }

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
        CurrentRoom = -1;
        RoomChanged = false;

        PlayerPosition3D = new Vector3(-4.05f,3.541f,28.56f);
        PlayerRotation3D = new Vector3(0f,180f,0f);
        CameraPosition3D = Vector3.zero;
        CameraRotation3D = Vector3.zero;

        Door0Unlocked = false;
        Door0AnimPlayed3D = false;
        Door0AnimPlayed2D = false;

        Door1Unlocked = false;
        Door1AnimPlayed3D = false;
        Door1AnimPlayed2D = false;

        PlayerHasUSB = false;
        USBInserted = false;

        PlayerPosition2D = new Vector3(0.13f, 2.1f, 0.0f);
        PlayerPuzzlePosition2D = new Vector3(-6.64f,-1.75f,0f);

        BatteriesCollected = 0;

        CurrentExtrudableSetId = -1;
        Extrudables = new List<bool> { false, false, false, false, false, false };
        ExtrudablesAnimPlayed2D = new List<bool> { false, false, false, false, false, false };

        TutorialLevelPorts = new List<bool> { false, false, false, false, false, false };
        ComputerLabLevelPorts = new List<bool> { false };

        BlueOverlayOn = false;
        PinkOverlayOn = false;

        BlueGroup0On = false;
        BlueGroup1On = false;
        BlueGroup2On = false;

        SceneName = "new3Dtut";
    }
}
