using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public GameObject leftClick;
    public GameObject pressQ;
    public GameObject pressSpace;
    public GameObject controls;

    // Start is called before the first frame update
    void Start()
    {
        leftClick = transform.Find("LeftClick").gameObject;
        pressQ = transform.Find("PressQ").gameObject;
        pressSpace = transform.Find("PressSpace").gameObject;
        controls = transform.Find("Controls").gameObject;

        leftClick.SetActive(false);
        pressQ.SetActive(false);
        pressSpace.SetActive(false);
        controls.SetActive(false);
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

    public void ShowSpaceTooltip()
    {
        if (pressSpace.activeSelf) return;
        StartCoroutine(ShowTooltip(pressSpace, 1f));
    }

    public void ActivateControls()
    {
        StartCoroutine(ShowTooltip(controls, 5f));
    }

    IEnumerator ShowTooltip(GameObject tooltip, float duration = 3f)
    {
        if (tooltip == controls)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Interaction>().enabled = false;
        }
        tooltip.SetActive(true);
        yield return new WaitForSeconds(duration);
        if (tooltip == controls)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Interaction>().enabled = true;
        }
        tooltip.SetActive(false);
    }
}
