using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackingSlider : MonoBehaviour
{
    public GameObject slider;
    // Start is called before the first frame update
    void Start()
    {
        slider = transform.Find("Slider").gameObject;
        slider.SetActive(false);
    }

    // Update is called once per frame
    public void ToggleSlider(bool toggle)
    {
        if (slider.activeSelf == toggle) return;
        slider.SetActive(toggle);
    }
}
