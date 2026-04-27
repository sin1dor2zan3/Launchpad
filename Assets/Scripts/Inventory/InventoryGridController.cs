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
        HandleDrop();
        UpdatePreview();
    }

    void HandleMovement()
    {
        Vector2 dir = Vector2.zero;

        if (Keyboard.current.wKey.wasPressedThisFrame) dir.y += 1;
        if (Keyboard.current.sKey.wasPressedThisFrame) dir.y -= 1;
        if (Keyboard.current.dKey.wasPressedThisFrame) dir.x += 1;
        if (Keyboard.current.aKey.wasPressedThisFrame) dir.x -= 1;

        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.leftStick.ReadValue();

            if (stick.magnitude > 0.5f)
                dir += new Vector2(Mathf.Round(stick.x), Mathf.Round(stick.y));
        }

        if (dir != Vector2.zero)
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                dir = new Vector2(Mathf.Sign(dir.x), 0);
            else
                dir = new Vector2(0, Mathf.Sign(dir.y));

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
        currentY -= dy;

        int rows = Mathf.CeilToInt((float)slots.Length / columns);

        currentX = Mathf.Clamp(currentX, 0, columns - 1);
        currentY = Mathf.Clamp(currentY, 0, rows - 1);

        currentIndex = currentY * columns + currentX;

        if (currentIndex >= slots.Length)
            currentIndex = slots.Length - 1;
    }

    void HandleRotation()
    {
        GameObject held = InventoryManager.Instance.GetHeldItem();
        if (held == null) return;

        bool rotate = Keyboard.current.qKey.wasPressedThisFrame;

        if (Gamepad.current != null)
            rotate |= Gamepad.current.yButton.wasPressedThisFrame;

        if (!rotate) return;

        InteractableObject item = held.GetComponent<InteractableObject>();

        int temp = item.width;
        item.width = item.height;
        item.height = temp;
    }

    void HandlePlacement()
    {
        bool place = Keyboard.current.spaceKey.wasPressedThisFrame ||
                     (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame);

        if (!place) return;

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
        InventoryManager.Instance.ToggleInventory();
    }

    void HandleDrop()
    {
        bool drop = Keyboard.current.rKey.wasPressedThisFrame ||
                    (Gamepad.current != null && Gamepad.current.bButton.wasPressedThisFrame);

        if (!drop) return;

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        if (InventoryManager.Instance.HasHeldItem())
        {
            InventoryManager.Instance.DropHeldItem(player);
            return;
        }

        InventorySlot slot = slots[currentIndex];

        if (!slot.isOccupied || slot.occupyingItem == null)
            return;

        GameObject item = slot.occupyingItem;

        RemoveFullItemFromGrid(item);

        item.SetActive(true);
        item.transform.position = player.position + player.forward * 2f;
    }

    void RemoveFullItemFromGrid(GameObject item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].occupyingItem == item)
                slots[i].ClearSlot();
        }
    }

    void UpdatePreview()
    {
        ClearAllHighlights();

        GameObject held = InventoryManager.Instance.GetHeldItem();

        if (held == null)
        {
            slots[currentIndex].SetHighlight(Color.yellow);
            return;
        }

        InteractableObject item = held.GetComponent<InteractableObject>();

        bool canPlace = CanPlaceItem(currentIndex, item);
        Color color = canPlace ? slots[0].highlightValid : slots[0].highlightInvalid;

        int startX = currentIndex % columns;
        int startY = currentIndex / columns;

        for (int y = 0; y < item.height; y++)
        {
            for (int x = 0; x < item.width; x++)
            {
                int gridX = startX + x;
                int gridY = startY + y;

                if (gridX >= columns)
                    continue;

                int index = gridY * columns + gridX;

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