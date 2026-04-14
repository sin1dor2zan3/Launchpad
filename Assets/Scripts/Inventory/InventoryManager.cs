using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject inventoryUI;

    private GameObject heldItem;
    private bool isOpen;

    private void Awake()
    {
        Instance = this;
        inventoryUI.SetActive(false);
        isOpen = false;
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryUI.SetActive(isOpen);

        Time.timeScale = isOpen ? 0f : 1f;
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    public bool HasHeldItem()
    {
        return heldItem != null;
    }

    public GameObject GetHeldItem()
    {
        return heldItem;
    }

    public void SetHeldItem(GameObject item)
    {
        heldItem = item;
    }

    public void ClearHeldItem()
    {
        heldItem = null;
    }

    public void DropHeldItem(Transform player)
    {
        if (heldItem == null) return;

        heldItem.SetActive(true);
        heldItem.transform.position = player.position + player.forward * 2f;

        heldItem = null;
    }
}