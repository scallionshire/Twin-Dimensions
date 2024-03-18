using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayControls : MonoBehaviour
{
    [SerializeField] private GameObject pinkoverlay;
    [SerializeField] private GameObject blueoverlay;
    public bool bluesolved;
    public bool pinksolved;
    // Start is called before the first frame update
    public void Start()
    {
        pinkoverlay.SetActive(false);
        blueoverlay.SetActive(false);
    }

    // Update is called once per frame
    public void turnOnOverlay()
    {
        if(pinksolved){
            pinkoverlay.SetActive(true);
            }
        if(bluesolved){
            blueoverlay.SetActive(true);
            }
    }
}
