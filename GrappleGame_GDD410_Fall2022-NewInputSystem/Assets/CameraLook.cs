using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour {

    public Transform lookAt;
    public Transform camTransform;
    public float sensitivityX = 4;
    public float sensitivityY = 4;
    public float Y_ANGLE_MIN = -70.0f;
    public float Y_ANGLE_MAX = 70.0f;
    public Camera cam;

    private float distance = 15;
    float currentX = 0;
    float currentY = 10;

    public PlayerInput playerControls;
    private InputAction camera;
    Vector2 canmeraInput = Vector2.zero;

    void Awake()
    {

        playerControls = new PlayerInput();
    }

    private void OnEnable()
    {
        camera = playerControls.Player.Camera;
        camera.Enable();
    }

    private void OnDisable()
    { 
        camera.Disable();
    }

    private void Start()
    {
        camTransform = transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        canmeraInput = camera.ReadValue<Vector2>();
        currentX += canmeraInput.x * sensitivityX;
        currentY -= canmeraInput.y * sensitivityY;

        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);

        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        camTransform.position = lookAt.position + rotation * dir;
        camTransform.LookAt(lookAt.position);
    }
}
