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

    private FMODUnity.StudioEventEmitter eventEmitter;
    private string currentCutscene;

    private Dictionary<string, int> cutsceneIndex = new Dictionary<string, int>
    {   
        {"intro", 0},
        {"switch", 1},
        {"outro", 2}
    };

    [SerializeField]
    private string[] videoFileNames;
    
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

        if (cutsceneName == "intro")
        {
            cutsceneMusicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/IntroCutscene");
            cutsceneMusicInstance.start();
            currentCutscene = "intro";
        } else if (cutsceneName == "switch")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX3D/TVOff", Camera.main.transform.position);
            currentCutscene = "switch";
        } else if (cutsceneName == "outro")
        {
            cutsceneMusicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/EndingMusic");
            cutsceneMusicInstance.start();
            currentCutscene = "outro";
        }

        videoPlayer.targetTexture.Create();
        videoImage.color = new Color(1, 1, 1, 1);
        int index = cutsceneIndex[cutsceneName];        

    #if UNITY_WEBGL
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileNames[index]);
        videoPlayer.url = videoPath;
    #else
        videoPlayer.clip = cutscenes[index];
    #endif
        videoPlayer.Play();
    }

    void OnCutsceneEnd(VideoPlayer vp)
    {
        currentCutscene = null;
        cutsceneMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        cutsceneMusicInstance.release();

        videoImage.color = new Color(0, 0, 0, 0);
        videoPlayer.targetTexture.Release();
        videoPlayer.Stop();
        videoPlayer.clip = null;
    }

    public void StopCutscene()
    {
        currentCutscene = null;
        cutsceneMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        cutsceneMusicInstance.release();

        videoImage.color = new Color(0, 0, 0, 0);
        videoPlayer.targetTexture.Release();
        videoPlayer.Stop();
        videoPlayer.clip = null;
    }

    public bool IsPlaying(string cutsceneName = null)
    {
        if (cutsceneName != null && currentCutscene != cutsceneName)
        {
            return false;
        }

        return videoPlayer.isPlaying;
    }

}
