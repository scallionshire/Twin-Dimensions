using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertBattery : MonoBehaviour
{
    [SerializeField] private List<GameObject> batteries;
    [SerializeField] private List<GameObject> batteryLights;
    [SerializeField] private List<GameObject> monitorScreens;
    private List<Renderer> batteryLightRenderers;
    private List<Renderer> monitorScreenRenderers;
    private int batteriesIn = 0;
    [SerializeField] private Material batteryPanelLitMaterial;
    [SerializeField] private Material monitorScreenLitMaterial;

    // // Start is called before the first frame update
    public void Start()
    {
        for (int i = 0; i < batteries.Count; i++)
        {
            batteries[i].SetActive(false);
            batteryLightRenderers.Add(batteryLights[i].GetComponent<Renderer>());
            monitorScreenRenderers.Add(monitorScreens[i].GetComponent<Renderer>());
        }
    }

    // Update is called once per frame
    public void insert()
    {
        int batteryCount = GameManager.instance.gameState.BatteriesCollected;
        Debug.Log("Current batteryCount: " + batteryCount);
        if (batteryCount > 0)
        {
            batteries[batteriesIn].SetActive(true);
            batteryLightRenderers[batteriesIn].material = batteryPanelLitMaterial;
            monitorScreenRenderers[batteriesIn].material = monitorScreenLitMaterial;
            batteriesIn++;
            GameManager.instance.gameState.BatteriesCollected--;
        }

        if (batteryCount == 5)
        {
            // TODO: update level success state in gamemanager
        }
    }
}
