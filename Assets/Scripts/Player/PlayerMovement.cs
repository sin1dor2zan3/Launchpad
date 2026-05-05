using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.8f;

    public int itemsNeededToFinish = 10;
    public int levelCount = 0;

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
        controller.Move(move * moveSpeed * Time.deltaTime);

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

            // Always return to hub after finishing a level
            if (currentScene != "Hub")
            {
                SceneManager.LoadScene("Hub");
                levelCount++;
            }
        }
        else
        {
            Debug.Log("Need more items before finishing level!");
        }
    }
}