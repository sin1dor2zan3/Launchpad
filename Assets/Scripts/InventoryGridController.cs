using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryGridController : MonoBehaviour
{
    public InventorySlot[] slots;
    public int columns = 8;

    private int currentIndex = 0;
    private int currentX;
    private int currentY;

    private Vector2 lastMoveDir = Vector2.zero;

    private void Start()
    {
        currentX = currentIndex % columns;
        currentY = currentIndex / columns;
    }

    private void Update()
    {
        if (!InventoryManager.Instance.IsOpen())
            return;

        HandleMovement();
        HandleRotation();
        HandlePlacement();
        UpdatePreview();
    }

    // 🎮 MOVEMENT (WASD + Arrow Keys + Left Stick + D-Pad FIXED)
    void HandleMovement()
    {
        Vector2 dir = Vector2.zero;

        // ⌨️ Keyboard (step-based like inventory cursor)
        if (Keyboard.current.wKey.wasPressedThisFrame) dir.y += 1;
        if (Keyboard.current.sKey.wasPressedThisFrame) dir.y -= 1;
        if (Keyboard.current.dKey.wasPressedThisFrame) dir.x += 1;
        if (Keyboard.current.aKey.wasPressedThisFrame) dir.x -= 1;

        // 🎮 Controller input
        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.leftStick.ReadValue();
            Vector2 dpad = Gamepad.current.dpad.ReadValue();

            Vector2 input = stick + dpad;

            // Deadzone prevents drift + corner lock
            if (input.magnitude > 0.5f)
            {
                input = new Vector2(
                    Mathf.Round(input.x),
                    Mathf.Round(input.y)
                );

                dir += input;
            }
        }

        if (dir != Vector2.zero)
        {
            // Convert to single direction (no diagonals)
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                dir = new Vector2(Mathf.Sign(dir.x), 0);
            else
                dir = new Vector2(0, Mathf.Sign(dir.y));

            // Prevent spam while holding same direction
            if (dir != lastMoveDir)
            {
                Move((int)dir.x, (int)dir.y);
                lastMoveDir = dir;
            }
        }
        else
        {
            lastMoveDir = Vector2.zero;
        }
    }

    void Move(int dx, int dy)
    {
        currentX += dx;
        currentY -= dy; // grid-friendly Y flip

        int rows = slots.Length / columns;

        currentX = Mathf.Clamp(currentX, 0, columns - 1);
        currentY = Mathf.Clamp(currentY, 0, rows - 1);

        currentIndex = currentY * columns + currentX;
    }

    // 🔄 ROTATE ITEM (Arrows + D-Pad)
    void HandleRotation()
    {
        bool rotate =
            Keyboard.current.leftArrowKey.wasPressedThisFrame ||
            Keyboard.current.rightArrowKey.wasPressedThisFrame;

        if (Gamepad.current != null)
        {
            rotate |= Gamepad.current.dpad.left.wasPressedThisFrame ||
                      Gamepad.current.dpad.right.wasPressedThisFrame;
        }

        if (rotate)
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
        if (Mouse.current.leftButton.wasPressedThisFrame ||
            Keyboard.current.enterKey.wasPressedThisFrame ||
            (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
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