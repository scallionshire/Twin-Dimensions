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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        dialogueCanvas = GameObject.Find("DialogueCanvas");

        if (dialogueCanvas == null)
        {
            // No dialogue canvas found
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
        dialogueCanvas = GameObject.Find("DialogueCanvas");

        if (dialogueCanvas == null)
        {
            // No dialogue canvas found
            noDialogueCanvas = true;
        } else {
            dialogue02 = dialogueCanvas.transform.Find("02").gameObject;
            dialogue20 = dialogueCanvas.transform.Find("20").gameObject;

            ToggleActive(false);
        }
    }

    void Update()
    {
        if (dialogueCanvas == null && !noDialogueCanvas)
        {
            ReloadCanvas();
        }

        if (dialogueActive && Input.GetKeyDown(KeyCode.Return) && finishedDisplayingText)
        {
            Debug.Log("Pressed enter");
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

    private void ReloadCanvas()
    {
        dialogueCanvas = GameObject.Find("DialogueCanvas");

        dialogue02 = dialogueCanvas.transform.Find("02").gameObject;
        dialogue20 = dialogueCanvas.transform.Find("20").gameObject;

        ToggleActive(false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        ToggleActive(true);

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
            EndDialogue();
            return;
        }

        Sentence sentence = sentences.Dequeue();

        switch (sentence.twin)
        {
            case Twin.Twin_02:
                ToggleActive(true, Twin.Twin_02);
                ToggleActive(false, Twin.Twin_20);
                TMP_Text target02 = dialogue02.GetComponentInChildren<TMP_Text>();
                StartCoroutine(TypeSentence(target02, sentence.text));
                break;
            case Twin.Twin_20:
                ToggleActive(true, Twin.Twin_20);
                ToggleActive(false, Twin.Twin_02);
                TMP_Text target20 = dialogue20.GetComponentInChildren<TMP_Text>();
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX2D/DialogStartSound");
                StartCoroutine(TypeSentence(target20, sentence.text));
                break;
        }
    }

    void EndDialogue()
    {
        ToggleActive(false);
    }

    IEnumerator TypeSentence(TMP_Text targetText, string sentence)
    {
        targetText.text = "";

        while (targetText.text.Length < sentence.Length)
        {
            // Skip to the end of the sentence
            if (Input.GetKeyDown(KeyCode.Return) && targetText.text.Length > 1)
            {
                targetText.text = sentence;
                finishedDisplayingText = true;
                yield break;
            }
            targetText.text += sentence[targetText.text.Length];
            yield return null;
        }

        finishedDisplayingText = true;
    }
}
