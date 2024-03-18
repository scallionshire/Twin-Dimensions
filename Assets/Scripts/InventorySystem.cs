using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySystem : MonoBehaviour
{
    public GameObject inventoryBackground; 
    public TextMeshProUGUI itemCountText;
    public GameObject batteryImage;
     private float originalHeight;

    private void Start()
    {
        originalHeight = inventoryBackground.GetComponent<RectTransform>().sizeDelta.y; // Store the original height
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        bool hasUSB = GameManager.instance.gameState.PlayerHasUSB;
        int batteryCount = GameManager.instance.gameState.BatteriesCollected;

        inventoryBackground.SetActive(hasUSB);

        if (hasUSB)
        {
            Debug.Log("USB is in the inventory");
        }

        if (batteryCount > 0) {
            RectTransform inventoryBackgroundRect = inventoryBackground.GetComponent<RectTransform>();
            inventoryBackgroundRect.sizeDelta = new Vector2(inventoryBackgroundRect.sizeDelta.x, 130);
            batteryImage.SetActive(true);
            Debug.Log($"Batteries in the inventory: {batteryCount}");
            itemCountText.text = $"x{batteryCount}";

        }
        else {
            RectTransform inventoryBackgroundRect = inventoryBackground.GetComponent<RectTransform>();
            inventoryBackgroundRect.sizeDelta = new Vector2(inventoryBackgroundRect.sizeDelta.x, originalHeight);
            batteryImage.SetActive(false);
            itemCountText.text = ""; 
        }
    }
}
