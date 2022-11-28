using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    float movmementMultiplier = 10f;
    [SerializeField] float airMultiplier = 0.4f;


    Vector3 moveDirection;
    Vector2 moveInput = Vector2.zero;

    public PlayerInput playerControls;
    
    public float rbDrag = 0f;
    public float groundDrag = .001f;

    Rigidbody rb;

    [Header("Jump")]
    bool isGrounded;
    float playerHeight = 1f;
    public float jumpForce = 5f;

    [Header("Camera")]
    Camera cam;
    public Transform lookAt;
    public Transform camTransform;
    private float distance = 10;

    float mouseX;
    float mouseY;

    float multiplier = 2f;

    float xRotation;
    float yRotation;
    Vector2 cameraInput = Vector2.zero;


    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.1f);
        ControlDrag();
    }
    #region Movement
    private void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        rb.drag = rbDrag;
    }
    
    public void OnMovement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movmementMultiplier, ForceMode.Acceleration);
        }
      
        if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movmementMultiplier *airMultiplier, ForceMode.Acceleration);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }
    #endregion 

    public void OnCamera(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>(); 
    }

    private void LateUpdate()
    {
        xRotation += cameraInput.x * multiplier;
        yRotation -= cameraInput.y * multiplier;

        yRotation = Mathf.Clamp(yRotation, -70f, 70f);
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(yRotation, xRotation, 0);
        camTransform.position = lookAt.position + rotation * dir;
        camTransform.LookAt(lookAt.position);
    }

}
