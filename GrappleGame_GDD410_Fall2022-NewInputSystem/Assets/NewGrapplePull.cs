using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class NewGrapplePull : MonoBehaviour
{
    [Header("References")]
    private GrappleGun gg;
    [SerializedField] public Transform cam;
    [SerializedField] public Transform gunTip;
    [SerializedField] public LayerMask whatIsGrappleable;
    [SerializedField] public LineRenderer lr;

    [Header("Grappling")]
    [SerializedField] public float maxGrappleDistance;
    [SerializedField] public float grappleDelayTime;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    [SerializedField] public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Check")]

    [SerializedField] public bool grappling;

    [HideInInspector]
    public AnimatorHandler animatorHandler;




    void Start()
    {
        gg = GetComponent<GrappleGun>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        animatorHandler.Initialize();
    }

    public void StartGrapple(InputAction.CallbackContext context)
    {
        if (grapplingCdTimer > 0) return;
        grappling = true;
        RaycastHit hit; 
        if(Physics.Raycast(cam.position,cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        
    }

    private void StopGrapple()
    {
        grappling = false;
        grapplingCdTimer = grapplingCd;
        lr.enabled = false;

    }
    // Update is called once per frame
    void Update()
    {
        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;


        
    }
    private void LateUpdate()
    {
        if (grappling)
            lr.SetPosition(0, gunTip.position);
            
      
    }
}
