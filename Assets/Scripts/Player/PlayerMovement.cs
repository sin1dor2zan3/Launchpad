using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Animator animator;

    public float moveSpeed = 5f;
    public float gravity = -9.8f;

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
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        // MOVE
        controller.Move(move * moveSpeed * Time.deltaTime);

        // ROTATE (smoothly face movement direction)
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

        float horizontalSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
        animator.SetFloat("Speed", horizontalSpeed);
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