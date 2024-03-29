using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public GameObject leftClick;
    public GameObject pressQ;
    public GameObject pressSpace;

    // Start is called before the first frame update
    void Start()
    {
        leftClick = transform.Find("LeftClick").gameObject;
        pressQ = transform.Find("PressQ").gameObject;
        pressSpace = transform.Find("PressSpace").gameObject;

        leftClick.SetActive(false);
        pressQ.SetActive(false);
        pressSpace.SetActive(false);
    }

    public void ToggleClickTooltip(bool toggle)
    {
        if (leftClick.activeSelf == toggle) return;
        leftClick.SetActive(toggle);
    }

    public void ShowQTooltip()
    {
        if (pressQ.activeSelf) return;
        StartCoroutine(ShowTooltip(pressQ));
    }

    public void ShowQTooltipPermanently() {
        if (pressQ.activeSelf) return;
        pressQ.SetActive(true);
    }

    public void RemoveQTooltip() {
        pressQ.SetActive(false);
    }

    public void ToggleSpaceTooltip(bool toggle)
    {
        if (pressSpace.activeSelf == toggle) return;
        pressSpace.SetActive(toggle);
    }

    IEnumerator ShowTooltip(GameObject tooltip)
    {
        tooltip.SetActive(true);
        yield return new WaitForSeconds(3f);
        tooltip.SetActive(false);
    }
}
