using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerTo2D : MonoBehaviour
{
    public string sceneToLoad = "Scene2D"; 

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}