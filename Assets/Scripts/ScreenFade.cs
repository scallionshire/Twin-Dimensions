using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    public Image fadeImage;
    public TextMeshProUGUI messageText;
    public float fadeDuration = 0.5f;

    void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        messageText.color = new Color(1, 0, 0, 0); 
    }

    public void TriggerCaughtEffect()
    {
        StartCoroutine(CaughtTransition());
    }

    public void TriggerReturnEffect()
    {
        StartCoroutine(ReturnTransition());
    }

    IEnumerator CaughtTransition()
    {
        for (float t = 0f; t <= fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            fadeImage.color = Color.Lerp(new Color(0, 0, 0, 0), Color.black, normalizedTime);
            messageText.color = Color.Lerp(new Color(1, 0, 0, 0), new Color(1, 0, 0, 1), normalizedTime);
            yield return null;
        }
    }
    IEnumerator ReturnTransition() {
        
        for (float t = 0f; t <= fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            //messageText.color = Color.Lerp(new Color(1, 0, 0, 1), new Color(1, 0, 0, 0), normalizedTime);
            messageText.color = new Color(1, 0, 0, 0); 
            fadeImage.color = Color.Lerp(Color.black, new Color(0, 0, 0, 0), normalizedTime);
            yield return null;
        }

    }

}