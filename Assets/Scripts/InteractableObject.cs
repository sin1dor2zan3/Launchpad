using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 2f;

    [Header("Game Progress")]
    public static int totalObjectsPickedUp = 0;

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

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
        if (CompareTag("Finish"))
        {
            return;
        }

        Debug.Log("Picked up: " + gameObject.name);
        totalObjectsPickedUp++;

        Destroy(gameObject);
    }
}