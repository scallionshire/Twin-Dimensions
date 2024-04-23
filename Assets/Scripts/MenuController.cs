using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();
    List<AsyncOperation> scenesToUnload = new List<AsyncOperation>();
    private bool gameStarted = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        GameManager.instance.ActiveSceneName = "StartMenu";
    }

    void OnEnable()
    {
        if (gameStarted)
        {
            UnloadGame();
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        Cursor.lockState = CursorLockMode.Locked;
        scenesToLoad.Add(SceneManager.LoadSceneAsync("GUI", LoadSceneMode.Additive));
        scenesToLoad.Add(SceneManager.LoadSceneAsync("new3Dtut", LoadSceneMode.Additive));
        scenesToLoad.Add(SceneManager.LoadSceneAsync("mainPuzzle", LoadSceneMode.Additive));
        scenesToLoad.Add(SceneManager.LoadSceneAsync("new2dtut", LoadSceneMode.Additive));

        var cutsceneManager = GameObject.Find("CutsceneManager").GetComponent<CutsceneManager>();
        cutsceneManager.PlayCutscene("intro");

        while(cutsceneManager.IsPlaying())
        {
        }

        StartCoroutine(LoadAndDeactivate(scenesToLoad));
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
        
        GameManager.instance.DeactivateScene("new2dtut");
        GameManager.instance.DeactivateScene("mainPuzzle");
        GameManager.instance.DeactivateScene("StartMenu");
        GameManager.instance.ActiveSceneName = "new3Dtut";

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("new3Dtut"));
        GameManager.instance.gameStarted = true;
    }

    public void UnloadGame()
    {
        Debug.Log("Unloading game");

        scenesToUnload.Add(SceneManager.UnloadSceneAsync("GUI"));
        scenesToUnload.Add(SceneManager.UnloadSceneAsync("new3Dtut"));
        scenesToUnload.Add(SceneManager.UnloadSceneAsync("mainPuzzle"));
        scenesToUnload.Add(SceneManager.UnloadSceneAsync("new2dtut"));

        StartCoroutine(Unload(scenesToUnload));
    }

    private IEnumerator<YieldInstruction> Unload(List<AsyncOperation> unloadOperations)
    {
        foreach (var unloadOp in unloadOperations)
        {
            while (!unloadOp.isDone)
            {
                yield return null;
            }
        }

        GameManager.instance.ActiveSceneName = "StartMenu";
        GameManager.instance.gameStarted = false;
        GameManager.instance.gameState = new GameState();
        GameManager.instance.firstSwitch = true;
        GameManager.instance.tutorialPuzzle = Instantiate(GameManager.instance.initTutorialPuzzle);
        GameManager.instance.computerPuzzle = Instantiate(GameManager.instance.initComputerPuzzle);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("StartMenu"));
    }

    public void OpenSettings()
    {
        GetComponent<CanvasGroup>().interactable = !GetComponent<CanvasGroup>().interactable;
        GameManager.instance.TogglePauseMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlaySFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX2D/MenuInteraction");
    }
}