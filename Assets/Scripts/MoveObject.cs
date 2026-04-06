using UnityEngine;
using UnityEngine.InputSystem;

public class MoveObject : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;           // Player transform
    public float grabDistance = 3f;    // Max distance to pick object
    public float holdDistance = 2f;    // Distance in front of player
    public float maxYOffset = 1f;      // Max up/down from player Y

    [Header("Drag Settings")]
    public LayerMask pickableLayer;    // Pickable objects
    public Material ghostMaterial;     // Transparent ghost
    public LayerMask collisionLayer;   // Layers to prevent movement through

    [Header("Rotation Settings")]
    public float rotationIncrement = 15f;

    private GameObject ghostObject;
    private GameObject pickedPrefab;
    private bool holdingObject = false;

    private float currentX = 0f;
    private float currentY = 0f;
    private float yOffset = 0f;

    void Update()
    {
        if (holdingObject)
        {
            HandleMovement();
            HandleRotation();
            HandleScroll();
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (!holdingObject)
                TryPickUp();
            else
                PlaceObject();
        }
    }

    void TryPickUp()
    {
        GameObject[] pickables = GameObject.FindGameObjectsWithTag("Pickable");

        GameObject closest = null;
        float closestDist = grabDistance;

        foreach (GameObject obj in pickables)
        {
            float dist = Vector3.Distance(player.position, obj.transform.position);
            if (dist <= grabDistance && dist < closestDist)
            {
                closest = obj;
                closestDist = dist;
            }
        }

        if (closest != null)
        {
            pickedPrefab = closest;

            // Create ghost
            ghostObject = Instantiate(pickedPrefab);
            SetGhostMaterial(ghostObject);
            pickedPrefab.SetActive(false);

            currentX = 0f;
            currentY = 0f;
            yOffset = 0f;
            holdingObject = true;

            Debug.Log("Picked up: " + pickedPrefab.name);
        }
        else
        {
            Debug.Log("No pickable object nearby");
        }
    }

    void PlaceObject()
    {
        pickedPrefab.transform.position = ghostObject.transform.position;
        pickedPrefab.transform.rotation = ghostObject.transform.rotation;
        pickedPrefab.SetActive(true);

        Destroy(ghostObject);
        ghostObject = null;
        pickedPrefab = null;
        holdingObject = false;
    }

    void HandleMovement()
    {
        // Desired position in front of player
        Vector3 desiredPos = player.position + player.forward * holdDistance + Vector3.up * yOffset;

        // Check collision before moving
        Collider[] hits = Physics.OverlapBox(desiredPos, ghostObject.transform.localScale / 2f, ghostObject.transform.rotation, collisionLayer);
        if (hits.Length == 0)
        {
            ghostObject.transform.position = desiredPos;
        }
    }

    void HandleScroll()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            yOffset += scroll * 0.1f; // adjust speed if needed
            yOffset = Mathf.Clamp(yOffset, -maxYOffset, maxYOffset);
        }
    }

    void HandleRotation()
    {
        // Base rotation matches player
        float baseYRotation = player.eulerAngles.y;

        // Q/E for fine adjustments
        if (Keyboard.current.qKey.wasPressedThisFrame)
            currentY += rotationIncrement;

        if (Keyboard.current.eKey.wasPressedThisFrame)
            currentX += rotationIncrement;

        currentX = Mathf.Repeat(currentX, 360f);
        currentY = Mathf.Repeat(currentY, 360f);

        ghostObject.transform.rotation = Quaternion.Euler(currentX, baseYRotation + currentY, 0f);
    }

    void SetGhostMaterial(GameObject obj)
    {
        foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
        {
            Material[] mats = new Material[r.materials.Length];
            for (int i = 0; i < mats.Length; i++)
                mats[i] = ghostMaterial;
            r.materials = mats;
        }
    }
}