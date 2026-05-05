using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 2f;

    [Header("Inventory Shape")]
    public int width = 1;
    public int height = 1;

    public static int totalObjectsPickedUp = 0;

    private Transform player;
    private PickupHighlight highlight;

    private bool isPlayerClose = false;
    private bool isPickedUp = false;

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;
        else
            Debug.LogError("PLAYER TAG NOT FOUND!");

        highlight = GetComponent<PickupHighlight>();

        if (highlight == null)
        {
            highlight = gameObject.AddComponent<PickupHighlight>();
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        isPlayerClose = distance <= interactDistance;

        // Highlight when player is close, unless item is already picked up
        if (isPlayerClose || isPickedUp)
        {
            highlight.TurnHighlightOn();
        }
        else
        {
            highlight.TurnHighlightOff();
        }

        bool interactPressed =
            (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);

        if (!interactPressed) return;

        Debug.Log($"Interact attempt | Distance: {distance}");

        if (isPlayerClose)
        {
            Interact();
        }
        else
        {
            Debug.Log("Too far to interact");
        }
    }

    void Interact()
    {
        if (CompareTag("Finish"))
        {
            Debug.Log("Finish object blocked interaction");
            return;
        }

        InventoryManager.Instance.SetHeldItem(gameObject);

        isPickedUp = true;
        highlight.TurnHighlightOn();

        InventoryManager.Instance.ToggleInventory();
        totalObjectsPickedUp++;

        Debug.Log("Interacted with: " + name);
    }

    public void RemoveHighlight()
    {
        isPickedUp = false;
        highlight.TurnHighlightOff();
    }
}