using UnityEngine;
using UnityEngine.InputSystem;

public class MoveObject : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public float grabDistance = 2f;
    public float holdDistance = 1f;

    [Header("Height Settings")]
    public float headOffset = 0.2f;

    [Header("Safety Offset")]
    public float forwardSafetyOffset = 0.5f;

    [Header("Drag Settings")]
    public Material ghostMaterial;

    [Header("Rotation Settings")]
    public float rotationIncrement = 15f;

    private GameObject ghostObject;
    private GameObject pickedPrefab;

    private bool holdingObject = false;

    private float currentX = 0f;
    private float currentY = 0f;

    private bool ghostMode = false;

    void Update()
    {
        if (holdingObject)
        {
            HandleMovement();
            HandleRotation();

            if (!ghostMode)
            {
                ghostMode = true;
                SetGhostColliders(true);
            }
        }

        HandlePickupPlace();
    }

    // ---------------- INPUT ----------------
    void HandlePickupPlace()
    {
        bool pickupPressed =
            Keyboard.current.spaceKey.wasPressedThisFrame ||
            (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame);

        if (!pickupPressed) return;

        if (!holdingObject)
            TryPickUp();
        else
            PlaceObject();
    }

    // ---------------- PICKUP ----------------
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

        if (closest == null) return;

        pickedPrefab = closest;

        ghostObject = Instantiate(pickedPrefab);
        SetGhostMaterial(ghostObject);

        pickedPrefab.SetActive(false);

        currentX = 0f;
        currentY = 0f;

        holdingObject = true;
        ghostMode = true;

        SetGhostColliders(true);

        Debug.Log("Picked up: " + pickedPrefab.name);
    }

    // ---------------- PLACE ----------------
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

    // ---------------- MOVEMENT ----------------
    void HandleMovement()
    {
        if (ghostObject == null) return;

        float topY = GetPlayerTopY() + headOffset;

        Vector3 safeForward = player.forward * forwardSafetyOffset;

        Vector3 holdOffset = player.forward * holdDistance;

        Vector3 desiredPos = new Vector3(
            player.position.x + holdOffset.x + safeForward.x,
            topY,
            player.position.z + holdOffset.z + safeForward.z
        );

        ghostObject.transform.position = desiredPos;
    }

    // ---------------- ROTATION ----------------
    void HandleRotation()
    {
        if (ghostObject == null) return;

        bool up =
            Keyboard.current?.upArrowKey.wasPressedThisFrame == true ||
            (Gamepad.current != null && Gamepad.current.dpad.up.wasPressedThisFrame);

        bool down =
            Keyboard.current?.downArrowKey.wasPressedThisFrame == true ||
            (Gamepad.current != null && Gamepad.current.dpad.down.wasPressedThisFrame);

        bool left =
            Keyboard.current?.leftArrowKey.wasPressedThisFrame == true ||
            (Gamepad.current != null && Gamepad.current.dpad.left.wasPressedThisFrame);

        bool right =
            Keyboard.current?.rightArrowKey.wasPressedThisFrame == true ||
            (Gamepad.current != null && Gamepad.current.dpad.right.wasPressedThisFrame);

        if (up) currentX += rotationIncrement;
        if (down) currentX -= rotationIncrement;
        if (left) currentY -= rotationIncrement;
        if (right) currentY += rotationIncrement;

        currentX = Mathf.Clamp(currentX, 0f, 180f);

        float finalY = player.eulerAngles.y + currentY;

        ghostObject.transform.rotation =
            Quaternion.Euler(currentX, finalY, 0f);
    }

    // ---------------- PLAYER HEIGHT ----------------
    float GetPlayerTopY()
    {
        Collider col = player.GetComponentInChildren<Collider>();
        if (col != null)
            return col.bounds.max.y;

        Renderer rend = player.GetComponentInChildren<Renderer>();
        if (rend != null)
            return rend.bounds.max.y;

        return player.position.y + 1.8f;
    }

    // ---------------- VISUALS ----------------
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

    // ---------------- PHYSICS FIX ----------------
    void SetGhostColliders(bool enableGhost)
    {
        if (ghostObject == null) return;

        foreach (Collider col in ghostObject.GetComponentsInChildren<Collider>())
            col.enabled = !enableGhost;

        Rigidbody rb = ghostObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = enableGhost;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}