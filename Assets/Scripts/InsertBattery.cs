using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertBattery : MonoBehaviour
{
    [SerializeField] private GameObject battery;
    [SerializeField] private GameObject batterylight;
    [SerializeField] private Material batInMaterial;
    [SerializeField] private Material litMaterial;
    private List<Renderer> batRenderer = new List<Renderer>();
    private Renderer batlightRenderer;
    public bool hasBattery;
    // // Start is called before the first frame update
    public void Start()
    {
        batRenderer.AddRange(GetComponentsInChildren<Renderer>());
        batlightRenderer=batterylight.GetComponent<Renderer>();
        
        // batteryTexture= GetTexture("Battery"+batID);
    }

    // Update is called once per frame
    public void insert()
    {
        int batteryCount = GameManager.instance.gameState.BatteriesCollected;
        Debug.Log(batteryCount);
        if (batteryCount==0){
            Debug.Log("go collect battery");
        }
        else{
            if(hasBattery){
                Debug.Log("already have battery");
            }
            if(hasBattery==false){
                hasBattery=true;
                GameManager.instance.gameState.BatteriesCollected--;
                // GameManager.UseBattery();
                foreach (Renderer rend in batRenderer)
                {
                    rend.material = batInMaterial;
                }
                batlightRenderer.material=litMaterial;
            }
        }
    }
}
