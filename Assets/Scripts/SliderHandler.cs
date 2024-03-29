using UnityEngine;
using UnityEngine.UI;

public class SliderHandler : MonoBehaviour
{
    private Slider musicSlider;
    private Slider dialogueSlider;
    private Slider sfxSlider;
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
        GameObject music = GameObject.Find("Music");
        GameObject dialogue = GameObject.Find("Dialogue");
        GameObject sfx = GameObject.Find("SFX");
        
        musicSlider = music.GetComponent<Slider>();
        if (musicSlider != null)
        {
            musicSlider.value = GameManager.instance.MusicVolume;
            musicSlider.onValueChanged.AddListener(delegate { MusicValueChangeCheck(); });
        }

        dialogueSlider = dialogue.GetComponent<Slider>();
        if (dialogueSlider != null)
        {
            dialogueSlider.value = GameManager.instance.DialogueVolume;
            dialogueSlider.onValueChanged.AddListener(delegate { DialogueValueChangeCheck(); });
        }

        sfxSlider = sfx.GetComponent<Slider>();
        if (sfxSlider != null)
        {
            sfxSlider.value = GameManager.instance.SFXVolume;
            sfxSlider.onValueChanged.AddListener(delegate { SFXValueChangeCheck(); });
        }
    }

    public void MusicValueChangeCheck()
    {
        GameManager.instance.MusicVolume = musicSlider.value;
    }

    public void DialogueValueChangeCheck()
    {
        GameManager.instance.DialogueVolume = dialogueSlider.value;
    }

    public void SFXValueChangeCheck()
    {
        GameManager.instance.SFXVolume = sfxSlider.value;
    }

    public void QuitSettings()
    {
        GameManager.instance.TogglePauseMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
