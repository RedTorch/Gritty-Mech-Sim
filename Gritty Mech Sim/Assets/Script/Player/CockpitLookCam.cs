using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CockpitLookCam : MonoBehaviour
{
    private Vector2 CurrLookRotation = new Vector2(0f,0f);
    private float LookSpeed = 3f;
    private bool isPiloting = true;
    [SerializeField] private Camera pilotPovCamera;
    private float pilotingFov = 60f;
    private float freelookFov = 75f;
    private float zoomedFov = 40f;
    private float targetFov = 60f;
    private RaycastHit hit;
    private float camVel;

    [SerializeField] private Image pilotFreelookCrosshairs;
    [SerializeField] private MechMovementController mechMoveController;
    [SerializeField] private Transform camRootOuter;
    [SerializeField] private Transform cameraTransform;
    private Vector2 freelookRotation = new Vector2(0f,0f);

    // Start is called before the first frame update
    void Start()
    {
        pilotFreelookCrosshairs.enabled = !isPiloting;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Tab)) {
            freelookRotation.x += Input.GetAxis("Mouse X") * LookSpeed;
            freelookRotation.y = freelookRotation.y - (Input.GetAxis("Mouse Y") * LookSpeed);// Mathf.Clamp(CurrLookRotation.y + (Input.GetAxis("Mouse Y") * LookSpeed),-80f,80f);

            targetFov = freelookFov;
            if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit)) {
                if(hit.collider.gameObject.tag == "InteractablePanel") {
                    targetFov = zoomedFov;
                }
            }
            pilotFreelookCrosshairs.enabled = true;
        } else {
            targetFov = pilotingFov;
            pilotFreelookCrosshairs.enabled = false;

            CurrLookRotation.x += Input.GetAxis("Mouse X") * LookSpeed;
            CurrLookRotation.y = Mathf.Clamp(CurrLookRotation.y + (Input.GetAxis("Mouse Y") * LookSpeed),-80f,80f);
            freelookRotation = new Vector2(Mathf.Lerp(freelookRotation.x,0f,10f * Time.deltaTime), Mathf.Lerp(freelookRotation.y,0f,10f * Time.deltaTime));
        }
        cameraTransform.localRotation = Quaternion.Euler(freelookRotation.y, freelookRotation.x,0f);
        camRootOuter.localRotation = Quaternion.Euler(0f,CurrLookRotation.x,0f);
        transform.localRotation = Quaternion.Euler(-1f * CurrLookRotation.y, 0f, 0f);
        pilotPovCamera.fieldOfView = Mathf.SmoothDamp(pilotPovCamera.fieldOfView, targetFov, ref camVel, 0.1f);
        print("FLR: (" + freelookRotation.x + ", " + freelookRotation.y + "),   CT.LR: (" + cameraTransform.localRotation.x + ", " + cameraTransform.localRotation.y + ", " + cameraTransform.localRotation.z + ")");
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

    public Vector2 getLookRotation() {
        return CurrLookRotation;
    }

    public bool getIsPiloting() {
        return isPiloting;
    }
}
