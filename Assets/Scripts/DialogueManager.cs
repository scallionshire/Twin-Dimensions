using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private Queue<Sentence> sentences;
    public GameObject dialogueCanvas;
    public GameObject dialogueBox_02;
    public GameObject dialogueBox_20;
    public TMP_Text dialogueText_02;
    public TMP_Text dialogueText_20;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<Sentence>();
        dialogueCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueCanvas.SetActive(true);

        sentences.Clear();

        foreach (Sentence sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        Sentence sentence = sentences.Dequeue();

        switch (sentence.twin)
        {
            case Twin.Twin_02:
                dialogueBox_02.SetActive(true);
                dialogueBox_20.SetActive(false);
                StartCoroutine(TypeSentence(dialogueText_02, sentence.text));
                break;
            case Twin.Twin_20:
                dialogueBox_02.SetActive(false);
                dialogueBox_20.SetActive(true);
                StartCoroutine(TypeSentence(dialogueText_20, sentence.text));
                break;
        }
    }

    void EndDialogue()
    {
        Debug.Log("End of conversation");
        dialogueCanvas.SetActive(false);
    }

    IEnumerator TypeSentence(TMP_Text targetText, string sentence)
    {
        targetText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            targetText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(2);
        DisplayNextSentence();
    }
}
