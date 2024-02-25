using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCanvas : MonoBehaviour
{
    public static CutsceneCanvas instance;
    private bool sceneLoaded = true;

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
}
