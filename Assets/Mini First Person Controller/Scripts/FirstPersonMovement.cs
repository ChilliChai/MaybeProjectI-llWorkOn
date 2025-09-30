using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonMovement : MonoBehaviour
{
// Movement Variables
public float speed = 5;
[Header("Running")]
public bool canRun = true;
public bool IsRunning { get; private set; }
public float runSpeed = 9;
public KeyCode runningKey = KeyCode.LeftShift;

// Look Variables
[Header("Look")]
public float mouseSensitivity = 100f;
public float lookUpLimit = 90f;
public float lookDownLimit = -90f;

// Internal Variables
private Rigidbody rb;
private float xRotation = 0f;
public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

void Awake()
{
// Get Rigidbody component
rb = GetComponent<Rigidbody>();
// Lock and hide the cursor
Cursor.lockState = CursorLockMode.Locked;
Cursor.visible = false;
}

void Update()
{
// Get mouse input
float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

// --- HORIZONTAL ROTATION (Player Body) ---
// Rotate the player's body left and right
transform.Rotate(Vector3.up * mouseX);

// --- VERTICAL ROTATION (Player Body) ---
// Calculate and clamp vertical rotation
xRotation -= mouseY;
xRotation = Mathf.Clamp(xRotation, lookDownLimit, lookUpLimit);
// Apply the rotation to the player's local transform
transform.localRotation = Quaternion.Euler(xRotation, transform.localRotation.eulerAngles.y, 0f);
}

void FixedUpdate()
{
// Determine movement speed
IsRunning = canRun && Input.GetKey(runningKey);
float targetMovingSpeed = IsRunning ? runSpeed : speed;
if (speedOverrides.Count > 0)
{
targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
}

// Get keyboard input
Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
// Convert local movement to world space based on player's rotation
Vector3 movement = new Vector3(input.x, 0, input.y);
Vector3 finalVelocity = transform.rotation * movement * targetMovingSpeed;

// Apply movement velocity
rb.velocity = new Vector3(finalVelocity.x, rb.velocity.y, finalVelocity.z);
}
}