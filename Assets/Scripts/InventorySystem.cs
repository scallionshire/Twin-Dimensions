using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem instance; 

    public GameObject inventoryUI; 
    public Text inventoryText; 

    private List<string> inventoryItems = new List<string>();
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        inventoryUI.SetActive(true); 
    }

    public void AddToInventory(string itemName)
    {
        inventoryItems.Add(itemName); 
        Debug.Log("Added USB to inventory");
        inventoryText.text = "Inventory: " + itemName;
    }
}