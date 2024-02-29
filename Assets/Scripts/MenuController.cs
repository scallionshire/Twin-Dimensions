using UnityEngine;
using UnityEngine.SceneManagement; 
public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("fbx3dtut");
    }
}