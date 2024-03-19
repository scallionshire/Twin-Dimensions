using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public GameObject leftClick;
    public GameObject pressQ;
    public GameObject pressSpace;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        leftClick = transform.Find("LeftClick").gameObject;
        pressQ = transform.Find("PressQ").gameObject;
        pressSpace = transform.Find("PressSpace").gameObject;

        leftClick.SetActive(false);
        pressQ.SetActive(false);
        pressSpace.SetActive(false);

        if (GameObject.Find("GameManager") != null)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
    }

    public void ToggleClickTooltip(bool toggle)
    {
        if (leftClick.activeSelf) return;
        leftClick.SetActive(toggle);
    }

    public void ShowQTooltip()
    {
        if (pressQ.activeSelf) return;
        StartCoroutine(ShowTooltip(pressQ));
    }

    public void ToggleSpaceTooltip(bool toggle)
    {
        if (pressSpace.activeSelf) return;
        pressSpace.SetActive(toggle);
    }

    IEnumerator ShowTooltip(GameObject tooltip)
    {
        tooltip.SetActive(true);
        if (tooltip.name == "PressQ") {
            yield return new WaitForSeconds(2f);
        } else {
            yield return new WaitForSeconds(1f);
        }
        tooltip.SetActive(false);
    }
}
