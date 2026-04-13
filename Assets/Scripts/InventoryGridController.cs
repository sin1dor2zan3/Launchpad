using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryGridController : MonoBehaviour
{
    public InventorySlot[] slots;
    public int columns = 8;

    private int currentIndex = 0;

    private void Update()
    {
        if (!InventoryManager.Instance.IsOpen())
            return;

        HandleMovement();
        HandleRotation();
        HandlePlacement();
        UpdatePreview();
    }

    // 🎮 MOVEMENT
    void HandleMovement()
    {
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) Move(1);
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) Move(-1);
        if (Keyboard.current.downArrowKey.wasPressedThisFrame) Move(columns);
        if (Keyboard.current.upArrowKey.wasPressedThisFrame) Move(-columns);
    }

    void Move(int amount)
    {
        currentIndex = Mathf.Clamp(currentIndex + amount, 0, slots.Length - 1);
    }

    // 🔄 ROTATE ITEM
    void HandleRotation()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            GameObject held = InventoryManager.Instance.GetHeldItem();
            if (held == null) return;

            InteractableObject item = held.GetComponent<InteractableObject>();

            int temp = item.width;
            item.width = item.height;
            item.height = temp;
        }
    }

    // 🟩 PREVIEW SHAPE
    void UpdatePreview()
    {
        ClearAllHighlights();

        GameObject held = InventoryManager.Instance.GetHeldItem();
        if (held == null) return;

        InteractableObject item = held.GetComponent<InteractableObject>();

        bool canPlace = CanPlaceItem(currentIndex, item);
        Color color = canPlace ? slots[0].highlightValid : slots[0].highlightInvalid;

        int startX = currentIndex % columns;
        int startY = currentIndex / columns;

        for (int y = 0; y < item.height; y++)
        {
            for (int x = 0; x < item.width; x++)
            {
                int index = (startY + y) * columns + (startX + x);

                if (index < slots.Length)
                    slots[index].SetHighlight(color);
            }
        }
    }

    void ClearAllHighlights()
    {
        foreach (var slot in slots)
            slot.ResetVisual();
    }

    // 📦 PLACE ITEM
    void HandlePlacement()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
        {
            GameObject held = InventoryManager.Instance.GetHeldItem();
            if (held == null) return;

            InteractableObject item = held.GetComponent<InteractableObject>();

            if (!CanPlaceItem(currentIndex, item))
            {
                Debug.Log("Can't place item here");
                return;
            }

            PlaceItem(currentIndex, item);
            InventoryManager.Instance.ClearHeldItem();
        }
    }

    // 🔍 CHECK FIT
    bool CanPlaceItem(int startIndex, InteractableObject item)
    {
        int startX = startIndex % columns;
        int startY = startIndex / columns;

        if (startX + item.width > columns)
            return false;

        for (int y = 0; y < item.height; y++)
        {
            for (int x = 0; x < item.width; x++)
            {
                int index = (startY + y) * columns + (startX + x);

                if (index >= slots.Length)
                    return false;

                if (slots[index].isOccupied)
                    return false;
            }
        }

        return true;
    }

    // ✅ APPLY PLACEMENT
    void PlaceItem(int startIndex, InteractableObject item)
    {
        int startX = startIndex % columns;
        int startY = startIndex / columns;

        for (int y = 0; y < item.height; y++)
        {
            for (int x = 0; x < item.width; x++)
            {
                int index = (startY + y) * columns + (startX + x);
                slots[index].SetOccupied(item.gameObject);
            }
        }

        item.gameObject.SetActive(false);
    }
}