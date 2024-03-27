using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMODUnity;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
    private Queue<Sentence> sentences;
    public GameObject dialogueCanvas;
    public GameObject dialogue02;
    public GameObject dialogue20;
    public bool dialogueActive = false;
    public bool noDialogueCanvas = false;
    public bool finishedDisplayingText = false;

    private string currentDialogueName;

    private GameManager gameManager;
    private TooltipManager tooltipManager;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dialogueCanvas = GameObject.Find("DialogueCanvas");

        if (dialogueCanvas == null)
        {
            noDialogueCanvas = true;
        } else {
            noDialogueCanvas = false;
            
            dialogue02 = dialogueCanvas.transform.Find("02").gameObject;
            dialogue20 = dialogueCanvas.transform.Find("20").gameObject;

            ToggleActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<Sentence>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (dialogueActive && Input.GetButtonDown("Fire1") && finishedDisplayingText)
        {
            DisplayNextSentence();
        }
    }

    private void ToggleActive(bool active)
    {
        dialogue02.transform.Find("02Dialogue").gameObject.SetActive(active);
        dialogue20.transform.Find("20Dialogue").gameObject.SetActive(active);
        dialogueActive = active;
    }

    private void ToggleActive(bool active, Twin twin)
    {
        switch (twin)
        {
            case Twin.Twin_02:
                dialogue02.transform.Find("02Dialogue").gameObject.SetActive(active);
                break;
            case Twin.Twin_20:
                dialogue20.transform.Find("20Dialogue").gameObject.SetActive(active);
                break;
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        // Turn off tooltips
        if (GameObject.Find("TooltipCanvas") != null)
        {
            tooltipManager = GameObject.Find("TooltipCanvas").GetComponent<TooltipManager>();
            tooltipManager.ToggleClickTooltip(false);
        }

        ToggleActive(true);
        gameManager?.ToggleDialogueFreeze(true);

        currentDialogueName = dialogue.dialogueName;

        sentences.Clear();

        foreach (Sentence sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        finishedDisplayingText = false;

        if (sentences.Count == 0)
        {
            if (currentDialogueName == "ComputerFirstPlug") {
                if (GameObject.Find("TooltipCanvas") != null)
                {
                    GameObject.Find("TooltipCanvas").GetComponent<TooltipManager>().ShowQTooltipPermanently();
                }
                ToggleActive(false);
                currentDialogueName = "";
            }
            else {
                EndDialogue();
            }
            return;
        }

        Sentence sentence = sentences.Dequeue();

        switch (sentence.twin)
        {
            case Twin.Twin_02:
                ToggleActive(true, Twin.Twin_02);
                ToggleActive(false, Twin.Twin_20);

                TMP_Text target02 = dialogue02.transform.GetChild(0).Find("DialogueText").GetComponent<TMP_Text>();
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX2D/3DDialogStartSound");
                StartCoroutine(TypeSentence(target02, sentence.text));
                break;
            case Twin.Twin_20:
                ToggleActive(true, Twin.Twin_20);
                ToggleActive(false, Twin.Twin_02);
                
                TMP_Text target20 = dialogue20.transform.GetChild(0).Find("DialogueText").GetComponent<TMP_Text>();
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX2D/2DDialogStartSound");
                StartCoroutine(TypeSentence(target20, sentence.text));
                break;
        }
    }

    public void EndDialogue()
    {
        ToggleActive(false);

        currentDialogueName = "";

        gameManager.ToggleDialogueFreeze(false);
    }

    IEnumerator TypeSentence(TMP_Text targetText, string sentence)
    {
        targetText.text = "";

        while (targetText.text.Length < sentence.Length)
        {
            // Skip to the end of the sentence
            if (Input.GetButtonDown("Fire1") && targetText.text.Length > 1)
            {
                targetText.text = sentence;
                finishedDisplayingText = true;
                yield break;
            }

            // Otherwise, continue typing
            targetText.text += sentence[targetText.text.Length];
            yield return null;
        }

        finishedDisplayingText = true;
    }
}
