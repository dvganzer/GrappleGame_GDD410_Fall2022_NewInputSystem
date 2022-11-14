using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    [SerializeField] private float maxDistance;
    private SpringJoint joint;

    PlayerInput playerControl;
    InputAction swing;

    void Awake()
    {
        playerControl = new PlayerInput();
        lr = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        swing = playerControl.Player.Swing;
        swing.Enable();
        swing.performed += StartGrapple;
        swing.canceled += StopGrapple;

    }
    private void OnDisable()
    {
        swing.Disable();

    }


    void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple(InputAction.CallbackContext context)
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

    void StopGrapple(InputAction.CallbackContext context)
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
        if(other.gameObject.name == "ToHigh")
        {
            swing.canceled += StopGrapple;

        }
    }
}



