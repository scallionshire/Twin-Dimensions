using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public VideoClip[] cutscenes;
    private VideoPlayer videoPlayer;
    public RenderTexture videoTexture;
    public RawImage videoImage;

    public static CutsceneManager instance;

    private Dictionary<string, int> cutsceneIndex = new Dictionary<string, int>
    {
        {"3DTwinVisor", 0}
        
    };
    
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

        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.loopPointReached += OnCutsceneEnd;
        videoPlayer.targetTexture = videoTexture;
        videoImage.color = new Color(0, 0, 0, 0);
    }

    public void PlayCutscene(string cutsceneName)
    {
        if (!cutsceneIndex.ContainsKey(cutsceneName))
        {
            Debug.LogError("Cutscene name not found.");
            return;
        }
        videoImage.color = new Color(1, 1, 1, 1);
        int index = cutsceneIndex[cutsceneName];
        Debug.Log("Playing cutscene: " + cutsceneName);
        videoPlayer.clip = cutscenes[index];
        videoPlayer.Play();

        while (videoPlayer.isPlaying)
        {
            Debug.Log("Cutscene is playing.");
        }
        Debug.Log("Cutscene done playing.");
    }

    void OnCutsceneEnd(VideoPlayer vp)
    {
        videoImage.color = new Color(0, 0, 0, 0);
        Debug.Log("Cutscene ended.");
    }

    public bool IsPlaying()
    {
        return videoPlayer.isPlaying;
    }
}
