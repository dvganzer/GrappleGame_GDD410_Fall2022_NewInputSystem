using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
   
    [Header("Jumping")]
    public float jumpForce = 5f;
    Vector3 moveDirection;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.2f;
    public bool isGrounded { get; private set; }


    Rigidbody rb;

    public float cameraMultiplier = .5f;

    float xRotation;
    float yRotation;
    Vector2 cameraInput = Vector2.zero;

    [Header("Swing")]
    [SerializeField] public LayerMask whatIsGrappleable;
    [SerializeField] public Transform gunTip;
    [SerializeField] public Transform camera;
    [SerializeField] public Transform player;
    private LineRenderer lr;
    private Vector3 grapplePoint;
    

    [SerializeField] private float maxDistance;

    private SpringJoint joint;

    [Header("Pull")]
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

    

    private Vector3 hookPoint;
    private void Start()
    {
        Debug.Log(isGrounded);
        //Movement
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        //Swing
        lr = GetComponent<LineRenderer>();
        //Pull 
        isShooting = false;
        isGrappling = false;
        lineRenderer.enabled = false;
        
    }

    private void Update()
    {
        //Movement && Jump
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
       
        //Swing
        if (Input.GetButtonDown("Fire1"))
        {
            StartGrapple();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            StopGrapple();
        }

       
    }

    private void LateUpdate()
    {
        //Swing
        DrawRope();
        //Pull 
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, _grapplingHookEndPoint.position);
            lineRenderer.SetPosition(1, _handPos.position);
        }
    }
    #region Movement
   
    
   


    public void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
       
    }
    #endregion

    #region Swing
    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);


            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;


            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }

    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;

    void DrawRope()
    {

        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "ToHigh")
        {
            StopGrapple();

        }
    }

    #endregion

    #region Pull
    public void OnPull()
    {
        
        if (_grapplingHook.parent == _handPos)
        {
            _grapplingHook.localPosition = new Vector3(1, 0, 0);
            _grapplingHook.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
        }
        if (Input.GetButtonDown("Fire2"))
        {
            ShootHook();
        }
        if (Input.GetButtonUp("Fire2"))
        {
            isGrappling = false;
            lineRenderer.enabled = false;
            _grapplingHook.position = _handPos.position;
            _grapplingHook.SetParent(_handPos);
        }

            if (isGrappling)
        {
            _grapplingHook.position = Vector3.Lerp(_grapplingHook.position, hookPoint, _hookSpeed * Time.deltaTime);
            if (Vector3.Distance(_grapplingHook.position, hookPoint) < 0.5f)
            {
                _playerBody.position = Vector3.Lerp(_playerBody.position, hookPoint - _offset, _hookSpeed * Time.deltaTime);

                if (Vector3.Distance(_playerBody.position, hookPoint) <= 7f)
                {

                    isGrappling = false;
                    Debug.Log(isGrappling);
                    _grapplingHook.SetParent(_handPos);
                    lineRenderer.enabled = false;
                }
            }


        }

    }
    private void ShootHook()
    {
        if (isShooting || isGrappling) return;
        isShooting = true;
        RaycastHit hit;

        if (Physics.Raycast(camera.position, camera.forward, out hit, _maxDistance, _grappleLayer))
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
    #endregion
}
