using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        scenesToLoad.Add(SceneManager.LoadSceneAsync("dialogue3DTut", LoadSceneMode.Additive));
        scenesToLoad.Add(SceneManager.LoadSceneAsync("mainPuzzle", LoadSceneMode.Additive));
        scenesToLoad.Add(SceneManager.LoadSceneAsync("new2dtut", LoadSceneMode.Additive));
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
        gameManager.ActiveSceneName = "dialogue3DTut";
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("dialogue3DTut"));
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