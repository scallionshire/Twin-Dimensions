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
    private FMOD.Studio.EventInstance cutsceneMusicInstance;

    private Dictionary<string, int> cutsceneIndex = new Dictionary<string, int>
    {   
        {"intro", 0},
        {"switch", 1}
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

        GameManager.instance.PauseMainMusic(true);

        cutsceneMusicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/IntroCutscene");
        cutsceneMusicInstance.start();

        videoImage.color = new Color(1, 1, 1, 1);
        int index = cutsceneIndex[cutsceneName];
        Debug.Log("Playing cutscene: " + cutsceneName);
        videoPlayer.clip = cutscenes[index];
        videoPlayer.Play();
    }

    void OnCutsceneEnd(VideoPlayer vp)
    {
        cutsceneMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        cutsceneMusicInstance.release();

        GameManager.instance.PauseMainMusic(false);
        videoImage.color = new Color(0, 0, 0, 0);
        Debug.Log("Cutscene ended.");
        videoPlayer.Stop();
        videoPlayer.clip = null;
    }

    public bool IsPlaying()
    {
        return videoPlayer.isPlaying;
    }
}
