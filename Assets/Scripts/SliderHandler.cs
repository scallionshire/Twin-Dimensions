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
        InitializeSliders();
    }

    private void InitializeSliders()
    {
        musicSlider = GameObject.Find("Music").GetComponent<Slider>();
        dialogueSlider = GameObject.Find("Dialogue").GetComponent<Slider>();
        sfxSlider = GameObject.Find("SFX").GetComponent<Slider>();
        
        if (musicSlider != null)
        {
            musicSlider.value = GameManager.instance.MusicVolume;
            musicSlider.onValueChanged.AddListener(delegate { MusicValueChangeCheck(); });
        }

        if (dialogueSlider != null)
        {
            dialogueSlider.value = GameManager.instance.DialogueVolume;
            dialogueSlider.onValueChanged.AddListener(delegate { DialogueValueChangeCheck(); });
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = GameManager.instance.SFXVolume;
            sfxSlider.onValueChanged.AddListener(delegate { SFXValueChangeCheck(); });
        }
    }

    public void MusicValueChangeCheck()
    {
        GameManager.instance.UpdateMusicVolume(musicSlider.value);
    }

    public void DialogueValueChangeCheck()
    {
        GameManager.instance.DialogueVolume = dialogueSlider.value;
    }

    public void SFXValueChangeCheck()
    {
        GameManager.instance.UpdateSFXVolume(sfxSlider.value);
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
