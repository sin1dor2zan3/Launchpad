using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryToggle : MonoBehaviour
{
    [Header("UI")]
    public GameObject inventoryUI; // Drag your Canvas here

    private bool isOpen = false;

    void Start()
    {
        inventoryUI.SetActive(false); // Start hidden
    }

    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryUI.SetActive(isOpen);

        // Optional: pause game when inventory is open
        Time.timeScale = isOpen ? 0f : 1f;

        // Optional: unlock/lock cursor
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }
}