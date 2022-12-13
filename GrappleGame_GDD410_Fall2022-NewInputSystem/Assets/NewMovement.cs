using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NewMovement : MonoBehaviour
{
    [Header("Camera")]
    [SerializedField] public Vector2 cameraInput = Vector2.zero;

    [SerializedField] private Vector2 currentInputVector;
    [SerializedField] private Vector2 smoothInputVelocity;
    


    [SerializedField] public float senseX =.5f;
    [SerializedField] public float senseY = .5f;

    [SerializedField] public Transform orientation;

    [SerializedField] public float xRotation;
    [SerializedField] public float yRotation;

    [Header("Movement")]
    [SerializedField] public Vector2 moveInput = Vector2.zero;

    [SerializedField] public float moveSpeed;
    [SerializedField] Vector3 moveDirection;
     Rigidbody rb;

    [Header("Ground Check")]
    [SerializedField] public float groundDrag;
    [SerializedField] public float airDrag;
    [SerializedField] public float playerHeight;
    [SerializedField] public LayerMask whatisGround;
    [SerializedField] public bool grounded;

    [Header("Jumping")]
    [SerializedField] public float jumpForce;
    [SerializedField] public float jumpCooldown;
    [SerializedField] public float airMultiplier;
    [SerializedField] public bool readyToJump = true;

    [Header("UI")]
    [SerializedField] public Text SenseText;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        SpeedControl();
        //Camera
       
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        float cameraX = cameraInput.x * senseX;
        float cameraY = cameraInput.y * senseY;

        yRotation += cameraX;
        xRotation -= cameraY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + .2f, whatisGround);
        if (grounded)
            rb.drag = groundDrag;
        else 
            rb.drag = airDrag;
       
    }
    private void FixedUpdate()
    {
        //Movement
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 5f, ForceMode.Force);
            rb.mass = 1;
        }


        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            rb.mass = 20;
        }




    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnCamera(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>() ;
        
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && readyToJump && grounded)
        {

            readyToJump = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce,  ForceMode.Impulse);
            rb.AddForce(transform.forward * jumpForce, ForceMode.Impulse);
            Invoke(nameof(ResetJump), jumpCooldown);

        }
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    public void SenseUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            senseX += .1f;
            senseY += .1f;
            SenseText.text = "SENSITIVITY:" + senseX *10;
        }

    }
    public void SenseDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            senseX -= .1f;
            senseY -= .1f;
            SenseText.text = "SENSITIVITY:" + senseX *10;
        }
            
    }
}
