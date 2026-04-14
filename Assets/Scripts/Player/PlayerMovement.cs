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

    private bool hasWon = false; // prevents multiple scene loads

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
        if (hasWon) return;

        if (other.CompareTag("Finish"))
        {
            if (InteractableObject.totalObjectsPickedUp >= 10)
            {
                hasWon = true;
                SceneManager.LoadScene("Win Screen");
            }
            else
            {
                Debug.Log("Need more items before winning!");
            }
        }
    }
}