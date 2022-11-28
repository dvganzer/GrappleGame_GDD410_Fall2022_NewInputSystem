using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplePull : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform _grapplingHookEndPoint;

    [SerializeField] private Transform _grapplingHook;
    [SerializeField] private Transform _handPos;
    [SerializeField] private Transform _playerBody;

    [SerializeField] private LayerMask _grappleLayer;

    [SerializeField] private float _maxDistance;
    [SerializeField] private float _hookSpeed;

    [SerializeField] private Vector3 _offset;

    private bool isShooting;
    private bool isGrappling;

    public Transform camera;

    private Vector3 hookPoint;

    public PlayerInput playerControl;
    private InputAction pull;

    private void Awake()
    {
        playerControl = new PlayerInput();
    }
    private void OnEnable()
    {
        pull = playerControl.Player.Pull;
        pull.Enable();
        //pull.performed += Pull;
        //pull.canceled += notPull;
    }
    private void OnDisable()
    {
        pull.Disable();

    }
    void Start()
    {
        isShooting = false;
        isGrappling= false;
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        pull.performed += Pull;
        pull.canceled += notPull;
    }

    void Pull(InputAction.CallbackContext context)
    {
        Debug.Log(Vector3.Distance(_playerBody.position, hookPoint - _offset));
        if(_grapplingHook.parent == _handPos)
        {
            _grapplingHook.localPosition = new Vector3(1,0,0);
            _grapplingHook.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
        }
   
       
            ShootHook();
       
        if (Input.GetButtonUp("Fire2"))
        {

        }

        if (isGrappling)
        {
            _grapplingHook.position = Vector3.Lerp(_grapplingHook.position, hookPoint, _hookSpeed * Time.deltaTime);
            if(Vector3.Distance(_grapplingHook.position, hookPoint) < 0.5f)
            {
                _playerBody.position = Vector3.Lerp(_playerBody.position, hookPoint - _offset, _hookSpeed * Time.deltaTime);

            }
            

        }
        if (Vector3.Distance(_playerBody.position, hookPoint) <= 7f)
        {

            isGrappling = false;
            Debug.Log(isGrappling);
            _grapplingHook.SetParent(_handPos);
            lineRenderer.enabled = false;
        }
    }
    public void notPull(InputAction.CallbackContext context)
    {
        isGrappling = false;
        lineRenderer.enabled = false;
        _grapplingHook.position = _handPos.position;
        _grapplingHook.SetParent(_handPos);

    }
    private void LateUpdate()
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, _grapplingHookEndPoint.position);
            lineRenderer.SetPosition(1, _handPos.position);
        }
    }
    private void ShootHook()
    {
        if (isShooting || isGrappling) return;
        isShooting = true;
        RaycastHit hit;
        
        if(Physics.Raycast(camera.position,camera.forward,out hit, _maxDistance, _grappleLayer))
        {
            hookPoint = hit.point;
            isGrappling = true;
            _grapplingHook.parent = null;
            _grapplingHook.LookAt(hookPoint);
            print("Hit!)");
            lineRenderer.enabled = true;
            Debug.Log(lineRenderer);
        }
        isShooting = false;

    }
}
