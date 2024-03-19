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
        leftClick.SetActive(toggle);
    }

    public void ShowQTooltip()
    {
        if (pressQ.activeSelf) return;
        StartCoroutine(ShowTooltip(pressQ));
    }

    public void ToggleSpaceTooltip(bool toggle)
    {
        pressSpace.SetActive(toggle);
    }

    IEnumerator ShowTooltip(GameObject tooltip)
    {
        tooltip.SetActive(true);
        yield return new WaitForSeconds(3f);
        tooltip.SetActive(false);
    }
}
