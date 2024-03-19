using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
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
        Debug.Log("Loading scenes");
        foreach (var loadOp in loadOperations)
        {   
            while (!loadOp.isDone)
            {
                yield return null; 
            }
        }

        Debug.Log("Scenes finished loading");

        var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.DeactivateScene("new2dtut");
        gameManager.DeactivateScene("mainPuzzle");
        gameManager.DeactivateScene("StartMenu");
        gameManager.ActiveSceneName = "new3Dtut";
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("new3Dtut"));
        gameManager.gameStarted = true;
    }

    public void OpenSettings()
    {
        Debug.Log("OpenSettings");
        GameObject.Find("GameManager").GetComponent<GameManager>().TogglePauseMenu();
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