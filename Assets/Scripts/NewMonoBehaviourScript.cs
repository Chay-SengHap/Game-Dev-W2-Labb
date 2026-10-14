using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [Header("Movement Settings")]
    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera (Optional)")]
    [SerializeField] private Transform cameraTransform; // Drag your Main Camera here if you want camera-relative movement

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // If cameraTransform isn't assigned, grab the main camera automatically
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        // Check if the character is touching the ground
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Get input from WASD or Arrow keys
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(moveX, 0, moveZ);

        // Make movement relative to the camera direction if a camera is assigned
        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            move = forward * moveZ + right * moveX;
        }

        // Move the character horizontally
        controller.Move(move * (playerSpeed * Time.deltaTime));

        // Rotate the character to face the movement direction smoothly
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Jump logic
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            // Physics formula for jump height: v = sqrt(height * -2 * gravity)
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        // Apply gravity over time
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Apply vertical movement (gravity/jumping)
        controller.Move(playerVelocity * Time.deltaTime);
    }
}