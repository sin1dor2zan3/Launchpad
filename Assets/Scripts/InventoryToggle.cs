using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI;

    private PlayerControls controls;
    private bool togglePressed;
    private bool isOpen;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Toggle.performed += ctx => togglePressed = true;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        inventoryUI.SetActive(false);
    }

    void Update()
    {
        if (!togglePressed) return;
        togglePressed = false;

        isOpen = !isOpen;

        inventoryUI.SetActive(isOpen);

        Time.timeScale = isOpen ? 0f : 1f;

        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }
}