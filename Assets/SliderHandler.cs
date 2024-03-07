using UnityEngine;
using UnityEngine.UI;

public class SliderHandler : MonoBehaviour
{
    private Slider musicSlider;
    private Slider dialogueSlider;
    private Slider sfxSlider;
    private GameManager GameManager;
    public static SliderHandler instance;

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
    
    void Start()
    {
        GameObject music = GameObject.Find("MUSIC");
        GameObject dialogue = GameObject.Find("DIALOGUE");
        GameObject sfx = GameObject.Find("SFX");
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        musicSlider = music.GetComponent<Slider>();
        if (musicSlider != null)
        {
            musicSlider.value = GameManager.MusicVolume;
            musicSlider.onValueChanged.AddListener(delegate { MusicValueChangeCheck(); });
        }

        dialogueSlider = dialogue.GetComponent<Slider>();
        if (dialogueSlider != null)
        {
            dialogueSlider.value = GameManager.DialogueVolume;
            dialogueSlider.onValueChanged.AddListener(delegate { DialogueValueChangeCheck(); });
        }

        sfxSlider = sfx.GetComponent<Slider>();
        if (sfxSlider != null)
        {
            sfxSlider.value = GameManager.SFXVolume;
            sfxSlider.onValueChanged.AddListener(delegate { SFXValueChangeCheck(); });
        }
    }

    public void MusicValueChangeCheck()
    {
        GameManager.MusicVolume = musicSlider.value;
    }

    public void DialogueValueChangeCheck()
    {
        GameManager.DialogueVolume = dialogueSlider.value;
    }

    public void SFXValueChangeCheck()
    {
        GameManager.SFXVolume = sfxSlider.value;
    }

    public void QuitSettings()
    {
        GameManager.TogglePauseMenu();
    }
}
