using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();
    private bool gameStarted = false;

    void Update()
    {
        if (!gameStarted) {
            Cursor.lockState = CursorLockMode.None;
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

    public void OpenSettings()
    {
        GameManager.instance.TogglePauseMenu();
    }

    public void OpenCredits()
    {
        // SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}