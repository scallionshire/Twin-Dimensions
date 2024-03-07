using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public bool hasObject = false;
    public GameObject inventoryImage; 

    public void AddObjectToInventory()
    {
        hasObject = true;
        UpdateInventoryUI();
    }

    private void UpdateInventoryUI()
    {
        if (hasObject)
        {
            Debug.Log("USB is in the inventory");
            inventoryImage.SetActive(true);
        }
        else
        {
            inventoryImage.SetActive(false);
        }
    }
}