using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechMovementController : MonoBehaviour
{
    private Vector3 CurrVelocity = new Vector3(0f,0f,0f);
    private float MoveSpeed = 12f;
    private Vector2 CurrLookRotation = new Vector2(0f,0f);
    private Rigidbody rb;
    [SerializeField] private GameObject lookRoot;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashDuration = 0.15f;
    private float dashSpeed = 50f;
    private Vector3 dashVector;
    [SerializeField] private AnimationCurve dashCurve;

    [SerializeField] private Animator camAnimator;

    [SerializeField] private CockpitLookCam pilotLookCam;
    [SerializeField] private Transform cockpitRotationRoot;
    [Tooltip("look speed of the mech camera root, in degrees per second")]
    private float lookSpeed = 90f;

    private float tiltFactor = 3f;
    private float currTilt = 0f;
    private float tiltVelocity = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if(camAnimator) {
            camAnimator.SetFloat("runSpeed", 0f);
        }
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // CurrLookRotation.x += Input.GetAxis("Mouse X") * LookSpeed;
        // CurrLookRotation.y = Mathf.Clamp(CurrLookRotation.y + (Input.GetAxis("Mouse Y") * LookSpeed),-80f,80f);
        if(pilotLookCam.getIsPiloting()) {
            Vector2 targetLook = pilotLookCam.getLookRotation();
            CurrLookRotation.x = Mathf.MoveTowardsAngle(CurrLookRotation.x, targetLook.x, lookSpeed*Time.deltaTime);
            CurrLookRotation.y = Mathf.Clamp(Mathf.LerpAngle(CurrLookRotation.y, targetLook.y, lookSpeed*Time.deltaTime),-30f,30f);
        }
        currTilt = Mathf.SmoothDamp(currTilt, (CurrVelocity.x/MoveSpeed)*(-1f)*tiltFactor, ref tiltVelocity, 0.2f);
        transform.localRotation = Quaternion.Euler(0f,CurrLookRotation.x,0f);
        lookRoot.transform.localRotation = Quaternion.Euler(-1f * CurrLookRotation.y, 0f, currTilt);
        cockpitRotationRoot.localRotation = Quaternion.Euler(-1f * CurrLookRotation.y,CurrLookRotation.x,0f);
        
        if(isDashing) {
            if(camAnimator) {
                camAnimator.SetFloat("runSpeed", 0f);
            }
            rb.velocity = dashVector * dashCurve.Evaluate(Mathf.Clamp(dashTimer/dashDuration,0f,1f));
            dashTimer -= Time.deltaTime;
            if(dashTimer <= 0) {
                isDashing = false;
            }
        }
        else {
            if(Input.GetButtonDown("Fire3")) {
                isDashing = true;
                dashTimer = dashDuration;
                dashVector = ((transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"))).normalized * dashSpeed;
            }
            CurrVelocity = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")) * MoveSpeed;
            if(camAnimator) {
                camAnimator.SetFloat("runSpeed", CurrVelocity.magnitude);
            }
            // rb.velocity = (transform.right * CurrVelocity.x) + (transform.forward * CurrVelocity.z);
            rb.AddForce(((transform.right * CurrVelocity.x) + (transform.forward * CurrVelocity.z) - rb.velocity)*5f);
        }
    }
}