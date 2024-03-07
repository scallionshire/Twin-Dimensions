using UnityEngine;
using UnityEngine.SceneManagement; 
public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("fbx3dtut");
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