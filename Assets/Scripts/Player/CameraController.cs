using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 2, -4);

    public float sensitivity = 2f;
    public float minY = -40f;
    public float maxY = 70f;

    [Header("Collision")]
    public float cameraRadius = 0.3f;
    public float minDistance = 0.5f;
    public LayerMask collisionMask;

    private PlayerControls controls;
    private Vector2 lookInput;

    private float yaw;
    private float pitch;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void LateUpdate()
    {
        // ROTATION INPUT
        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 targetPosition = player.position + Vector3.up * 1.5f;
        Vector3 desiredCameraPos = targetPosition + rotation * offset;

        Vector3 direction = (desiredCameraPos - targetPosition).normalized;
        float distance = Vector3.Distance(targetPosition, desiredCameraPos);

        RaycastHit hit;

        // SPHERECAST to prevent clipping
        if (Physics.SphereCast(targetPosition, cameraRadius, direction, out hit, distance, collisionMask))
        {
            float adjustedDistance = Mathf.Max(hit.distance, minDistance);
            transform.position = targetPosition + direction * adjustedDistance;
        }
        else
        {
            transform.position = desiredCameraPos;
        }

        // LOOK AT PLAYER
        transform.LookAt(targetPosition);
    }
}