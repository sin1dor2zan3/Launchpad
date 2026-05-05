using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float gravity = -9.8f;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Progression")]
    public int itemsNeededToFinish = 10;
    public static int levelCount = 0;
    public static bool[] levelCompleted = new bool[4];

    private CharacterController controller;
    private PlayerControls controls;

    private Vector2 moveInput;
    private Vector3 velocity;

    private bool isTransitioning = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        // CAMERA-RELATIVE MOVEMENT
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * moveInput.y + right * moveInput.x;

        controller.Move(move * moveSpeed * Time.deltaTime);

        // ROTATE PLAYER TOWARD MOVEMENT
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }

        // GRAVITY
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTransitioning) return;

        if (!other.CompareTag("Finish")) return;

        if (InteractableObject.totalObjectsPickedUp >= itemsNeededToFinish)
        {
            isTransitioning = true;

            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene != "Hub")
            {
                SceneManager.LoadScene("Hub");
                levelCount++;
                levelCompleted[levelCount - 1] = true;
            }
        }
        else
        {
            Debug.Log("Need more items before finishing level!");
        }
    }
}