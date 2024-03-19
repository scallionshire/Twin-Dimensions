using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertBattery : MonoBehaviour
{
    [SerializeField] private List<GameObject> batteries;
    [SerializeField] private List<GameObject> batteryLights;
    [SerializeField] private List<GameObject> monitorScreens;
    private List<MeshRenderer> batteryLightRenderers = new List<MeshRenderer>();
    private List<MeshRenderer> monitorScreenRenderers = new List<MeshRenderer>();
    private int batteriesIn = 0;
    [SerializeField] private Material batteryPanelLitMaterial;
    [SerializeField] private Material monitorScreenLitMaterial;

    // // Start is called before the first frame update
    public void Start()
    {
        for (int i = 0; i < batteries.Count; i++)
        {
            batteries[i].SetActive(false);
            if (batteryLights[i] == null)
            {
                Debug.Log("Battery light " + i + " is null");
            }
            if (batteryLights[i].GetComponent<MeshRenderer>() == null)
            {
                Debug.Log("Battery light " + i + " has no mesh renderer");
            }
            batteryLightRenderers.Add(batteryLights[i].GetComponent<MeshRenderer>());
            monitorScreenRenderers.Add(monitorScreens[i].GetComponent<MeshRenderer>());
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
            //GameManager.instance.gameState.BatteriesCollected--;\
            GameManager.instance.UseBattery();
        }

        if (batteriesIn == 5)
        {
            // TODO: update level success state in gamemanager
            GameManager.instance.SolvePuzzle(Level.computerlab, 1);
            GameObject.Find("PinkOverlay").GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
