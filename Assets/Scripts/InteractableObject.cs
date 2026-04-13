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

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;
        else
            Debug.LogError("PLAYER TAG NOT FOUND!");
    }

    void Update()
    {
        if (player == null) return;

        bool interactPressed =
            (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);

        if (!interactPressed) return;

        float distance = Vector3.Distance(transform.position, player.position);

        Debug.Log($"Interact attempt | Distance: {distance}");

        if (distance <= interactDistance)
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

        InventoryManager.Instance.OpenInventory(gameObject);
        totalObjectsPickedUp++;

        Debug.Log("Interacted with: " + name);
    }
}