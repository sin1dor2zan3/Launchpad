using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 2f; // Max distance to interact
    public string playerTag = "Player"; // Player tag to detect

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance <= interactDistance)
                {
                    Interact();
                }
            }
        }
    }

    void Interact()
    {
        Debug.Log("Interacted with: " + gameObject.name);
        Destroy(gameObject);
    }
}