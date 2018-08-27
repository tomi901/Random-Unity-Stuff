using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class FPSPlayerMovement : MonoBehaviour
{

    public float speed = 3f;

    public float xSensitivity = 1f;
    public float ySensitivity = 1f;

    public Camera playerCamera;


    CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Player y rotation
        transform.Rotate(Vector3.up * Input.GetAxis("LookHorizontal") * xSensitivity * Time.deltaTime, Space.Self);

        // Camera x rotation
        Vector3 cameraEuler = playerCamera.transform.localEulerAngles;
        float xRotation = (cameraEuler.x > 180) ? cameraEuler.x - 360 : cameraEuler.x;
        xRotation -= Input.GetAxis("LookVertical") * ySensitivity * Time.deltaTime;

        playerCamera.transform.localEulerAngles = new Vector3(Mathf.Clamp(xRotation, -90f, 90f), 0);
    }

    private void FixedUpdate()
    {
        // Player movement
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveInput = transform.TransformDirection(moveInput);
        moveInput *= speed;

        // Vertical or gravity movement
        float vSpeed = controller.velocity.y;
        vSpeed += Physics.gravity.y * Time.fixedDeltaTime;

        // Apply movement
        Vector3 moveVector = new Vector3(moveInput.x, vSpeed, moveInput.z);
        controller.Move(moveVector * Time.fixedDeltaTime);
    }
}
