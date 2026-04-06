using UnityEngine;
using UnityEngine.InputSystem;

public class MoveObject : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public float grabDistance = 3f;
    public float holdDistance = 2f;
    public float maxYOffset = 1f;

    [Header("Drag Settings")]
    public Material ghostMaterial;

    [Header("Rotation Settings")]
    public float rotationIncrement = 15f;

    private GameObject ghostObject;
    private GameObject pickedPrefab;
    private bool holdingObject = false;

    private float currentX = 0f; // X-axis rotation (tilt)
    private float currentY = 0f; // Y-axis rotation (spin)
    private float yOffset = 0f;   // Vertical offset
    private Vector3 offsetFromPlayer;

    private bool ghostMode = false;

    void Update()
    {
        if (holdingObject)
        {
            HandleMovement();
            HandleRotation();
            HandleScroll();

            // Keep ghost mode active while holding
            if (!ghostMode)
            {
                ghostMode = true;
                SetGhostColliders(true);
            }
        }

        // Pick up or place object with Left Mouse Button
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!holdingObject)
                TryPickUp();
            else
                PlaceObject();
        }

        // Orbit around player with Right Mouse Button
        if (holdingObject && Mouse.current.rightButton.wasPressedThisFrame)
        {
            offsetFromPlayer = Quaternion.Euler(0, 90f, 0) * offsetFromPlayer;
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
            ghostObject = Instantiate(pickedPrefab);
            SetGhostMaterial(ghostObject);
            pickedPrefab.SetActive(false);

            currentX = 0f;
            currentY = 0f;
            yOffset = 0f;
            holdingObject = true;

            offsetFromPlayer = player.forward * holdDistance;
            ghostObject.transform.position = player.position + offsetFromPlayer;

            ghostMode = true;
            SetGhostColliders(true);

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

        ghostMode = false;
    }

    void HandleMovement()
    {
        if (ghostObject == null) return;

        Vector3 desiredPos = player.position + offsetFromPlayer + Vector3.up * yOffset;
        ghostObject.transform.position = desiredPos;
    }

    void HandleScroll()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0)
        {
            yOffset += scroll * 0.1f;
            yOffset = Mathf.Clamp(yOffset, -maxYOffset, maxYOffset);
        }
    }

    void HandleRotation()
    {
        if (ghostObject == null) return;

        float baseYRotation = player.eulerAngles.y;

        // Up/Down arrows rotate X-axis
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            currentX += rotationIncrement;
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            currentX -= rotationIncrement;

        // Left/Right arrows rotate Y-axis
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            currentY -= rotationIncrement;
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            currentY += rotationIncrement;

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

    void SetGhostColliders(bool enableGhost)
    {
        if (ghostObject == null) return;

        Collider[] colliders = ghostObject.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
            col.enabled = !enableGhost;

        Rigidbody rb = ghostObject.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = enableGhost;
    }
}