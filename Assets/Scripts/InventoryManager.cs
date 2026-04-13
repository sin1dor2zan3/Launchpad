using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject inventoryUI;

    private GameObject heldItem;

    private void Awake()
    {
        Instance = this;
        inventoryUI.SetActive(false);
    }

    public void OpenInventory(GameObject item)
    {
        inventoryUI.SetActive(true);
        heldItem = item;

        Time.timeScale = 0f;
    }

    public bool HasHeldItem()
    {
        return heldItem != null;
    }

    public GameObject GetHeldItem()
    {
        return heldItem;
    }

    public void ClearHeldItem()
    {
        heldItem = null;
        CloseInventory();
    }

    public void DropItem(Transform player)
    {
        if (heldItem != null)
        {
            heldItem.transform.position = player.position + player.forward * 2f;
            heldItem.SetActive(true);

            heldItem = null;
            CloseInventory();
        }
    }

    public void CloseInventory()
    {
        inventoryUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public bool IsOpen()
    {
        return inventoryUI.activeSelf;
    }
}