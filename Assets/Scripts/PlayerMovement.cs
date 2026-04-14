using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.8f;

    private CharacterController controller;
    private PlayerControls controls;

    private Vector2 moveInput;
    private Vector3 velocity;
    private bool dropPressed;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Toggle.performed += ctx => dropPressed = true;
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

        if (dropPressed)
        {
            dropPressed = false;
            InventoryManager.Instance.DropItem(transform);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Finish"))
        {
            if (InteractableObject.totalObjectsPickedUp >= 9)
            {
                SceneManager.LoadScene("Win Screen");
            }
        }
    }
}