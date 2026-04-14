using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryToggle : MonoBehaviour
{
    private void Update()
    {
        bool toggle =
            Keyboard.current.tabKey.wasPressedThisFrame;

        if (Gamepad.current != null)
            toggle |= Gamepad.current.xButton.wasPressedThisFrame;

        if (toggle)
        {
            InventoryManager.Instance.ToggleInventory();
        }
    }
}