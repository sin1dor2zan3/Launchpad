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

    private float currentX = 0f;
    private float currentY = 0f;
    private float yOffset = 0f;

    private Vector3 offsetFromPlayer;

    private bool ghostMode = false;
    private int orbitIndex = 0;

    void Update()
    {
        if (holdingObject)
        {
            HandleMovement();
            HandleRotation();
            HandleScroll();

            if (!ghostMode)
            {
                ghostMode = true;
                SetGhostColliders(true);
            }
        }

        HandlePickupPlace();
        HandleOrbit();
    }

    // ---------------- INPUT: PICKUP / PLACE ----------------
    void HandlePickupPlace()
    {
        bool pickupPressed =
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame);

        if (!pickupPressed) return;

        if (!holdingObject)
            TryPickUp();
        else
            PlaceObject();
    }

    // ---------------- INPUT: ORBIT (FIXED) ----------------
    void HandleOrbit()
    {
        if (!holdingObject) return;

        bool orbitPressed =
            (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.leftTrigger.wasPressedThisFrame);

        if (!orbitPressed) return;

        // 🔥 true 4-direction orbit system
        orbitIndex = (orbitIndex + 1) % 4;

        Vector3 dir = Vector3.zero;

        switch (orbitIndex)
        {
            case 0: dir = player.forward; break;
            case 1: dir = player.right; break;
            case 2: dir = -player.forward; break;
            case 3: dir = -player.right; break;
        }

        offsetFromPlayer = dir.normalized * holdDistance;
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

            // start forward
            offsetFromPlayer = player.forward * holdDistance;
            orbitIndex = 0;

            ghostObject.transform.position = player.position + offsetFromPlayer;

            ghostMode = true;
            SetGhostColliders(true);

            Debug.Log("Picked up: " + pickedPrefab.name);
        }
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

        Vector3 desiredPos =
            player.position +
            offsetFromPlayer +
            Vector3.up * yOffset;

        ghostObject.transform.position = desiredPos;
    }

    // ---------------- HEIGHT CONTROL (FIXED RIGHT STICK) ----------------
    void HandleScroll()
    {
        float scroll = 0f;

        if (Mouse.current != null)
            scroll += Mouse.current.scroll.ReadValue().y;

        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.rightStick.ReadValue();

            float stickScroll = 0f;

            if (Mathf.Abs(stick.y) > 0.4f)
            {
                stickScroll = Mathf.Sign(stick.y) *
                              Mathf.Pow(Mathf.Abs(stick.y), 4f);
            }

            scroll += stickScroll * 1.5f;
        }

        if (scroll != 0)
        {
            yOffset += scroll * 0.1f;
            yOffset = Mathf.Clamp(yOffset, -maxYOffset, maxYOffset);
        }
    }

    // ---------------- ROTATION ----------------
    void HandleRotation()
    {
        if (ghostObject == null) return;

        float baseYRotation = player.eulerAngles.y;

        bool up =
            Keyboard.current?.upArrowKey.wasPressedThisFrame == true ||
            Gamepad.current?.dpad.up.wasPressedThisFrame == true;

        bool down =
            Keyboard.current?.downArrowKey.wasPressedThisFrame == true ||
            Gamepad.current?.dpad.down.wasPressedThisFrame == true;

        bool left =
            Keyboard.current?.leftArrowKey.wasPressedThisFrame == true ||
            Gamepad.current?.dpad.left.wasPressedThisFrame == true;

        bool right =
            Keyboard.current?.rightArrowKey.wasPressedThisFrame == true ||
            Gamepad.current?.dpad.right.wasPressedThisFrame == true;

        if (up) currentX += rotationIncrement;
        if (down) currentX -= rotationIncrement;
        if (left) currentY -= rotationIncrement;
        if (right) currentY += rotationIncrement;

        currentX = Mathf.Repeat(currentX, 360f);
        currentY = Mathf.Repeat(currentY, 360f);

        ghostObject.transform.rotation =
            Quaternion.Euler(currentX, baseYRotation + currentY, 0f);
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

    void SetGhostColliders(bool enableGhost)
    {
        if (ghostObject == null) return;

        foreach (Collider col in ghostObject.GetComponentsInChildren<Collider>())
            col.enabled = !enableGhost;

        Rigidbody rb = ghostObject.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = enableGhost;
    }
}