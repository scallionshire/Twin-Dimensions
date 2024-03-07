using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public GameObject pressE;
    public GameObject pressQ;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        pressE = transform.Find("PressE").gameObject;
        pressQ = transform.Find("PressQ").gameObject;

        pressE.SetActive(false);
        pressQ.SetActive(false);

        if (GameObject.Find("GameManager") != null)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
    }

    public void ShowETooltip()
    {
        if (gameManager.gameState.PressETooltipShown)
        {
            return;
        }
        StartCoroutine(ShowTooltip(pressE.GetComponent<Image>()));
        gameManager.gameState.PressETooltipShown = true;
    }

    public void ShowQTooltip()
    {
        if (gameManager.gameState.PressQTooltipShown || !gameManager.gameState.USBInserted)
        {
            return;
        }
        StartCoroutine(ShowTooltip(pressQ.GetComponent<Image>()));
        gameManager.gameState.PressQTooltipShown = true;
    }

    IEnumerator ShowTooltip(Image tooltip)
    {
        tooltip.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        tooltip.gameObject.SetActive(false);
    }
}
