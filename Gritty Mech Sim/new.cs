using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* USAGE: 
1) Create an object like so:
    Capsule > CameraRoot (empty transform) > Main Camera
    and add a Rigidbody to the Capsule!
2) Attach this script to the capsule.
3) Add CameraRoot to the script.
*/

public class FPController : MonoBehaviour
{
    private Vector3 TargetVelocity = new Vector3(0f,0f,0f);
    private Vector3 CurrVelocity = new Vector3(0f,0f,0f);
    private float MoveSpeed = 12f;
    private float LookSpeed = 3f;
    private Vector2 CurrLookRotation = new Vector2(0f,0f);
    private Rigidbody rb;
    [SerializeField] private GameObject camroot;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashDuration = 0.15f;
    private float dashSpeed = 75f;
    private Vector3 dashVector;
    [SerializeField] private AnimationCurve dashCurve;

    [SerializeField] private Animator camAnimator;
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
        CurrLookRotation.x += Input.GetAxis("Mouse X") * LookSpeed;
        CurrLookRotation.y = Mathf.Clamp(CurrLookRotation.y + (Input.GetAxis("Mouse Y") * LookSpeed),-80f,80f);
        transform.localRotation = Quaternion.Euler(0f,CurrLookRotation.x,0f);
        camroot.transform.localRotation = Quaternion.Euler(-1f * CurrLookRotation.y, 0f, 0f);
        
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
            rb.AddForce((transform.right * CurrVelocity.x) + (transform.forward * CurrVelocity.z) - rb.velocity);
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if(hasFocus) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else {
            // pause game, etc..
        }
    }
}
