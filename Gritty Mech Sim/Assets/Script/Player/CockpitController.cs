using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CockpitController : MonoBehaviour
{
    private Vector2 CurrLookRotation = new Vector2(0f,0f);
    private float LookSpeed = 3f;
    private bool isPiloting = true;
    [SerializeField] private Camera pilotPovCamera;
    private float pilotingFov = 65f;
    private float freelookFov = 75f;
    private float zoomedFov = 45f;
    private float targetFov = 62f;
    private RaycastHit hit;
    private float camVel;

    [SerializeField] private Image pilotFreelookCrosshairs;
    [SerializeField] private MechMovementController mechMoveController;
    [SerializeField] private LauncherController myLauncherController;
    [SerializeField] private LauncherController myRocketLauncherController;
    [SerializeField] private Transform camRootOuter;
    [SerializeField] private Transform cameraTransform;
    private Vector2 freelookRotation = new Vector2(0f,0f);

    [SerializeField] private AnimationCurve screenshakeAnimationCurve;
    private Vector3 camshakeRotOffset = new Vector3(0f,0f,0f);
    private Vector3 camshakeRotGoal = new Vector3(0f,0f,0f);
    private float singleShakeTime = 0f; // time it takes for a single shake to complete (from one point to another)
    private float intensity = 0f;
    private float currShakePercent = 0f;
    private float currShakeElapseFactor = 0f;
    private bool isShaking = false;
    private float camSpeedByFrequency = 0f;
    private Vector3 currShakeV = new Vector3();

    [SerializeField] private GameObject adsImage;

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
                if(Input.GetButtonDown("Fire1")) {
                    if(hit.collider.gameObject.GetComponent<InteractableButton>() != null) {
                        hit.collider.gameObject.GetComponent<InteractableButton>().onPressed();
                    }
                }
            }
            pilotFreelookCrosshairs.enabled = true;
            mechMoveController.setCurrMoveInput();
            mechMoveController.setIsFiring();
        } else {
            targetFov = pilotingFov;
            pilotFreelookCrosshairs.enabled = false;

            CurrLookRotation.x += Input.GetAxis("Mouse X") * LookSpeed;
            CurrLookRotation.y = Mathf.Clamp(CurrLookRotation.y + (Input.GetAxis("Mouse Y") * LookSpeed),-80f,80f);
            freelookRotation = new Vector2(Mathf.Lerp(freelookRotation.x,0f,10f * Time.deltaTime), Mathf.Lerp(freelookRotation.y,0f,10f * Time.deltaTime));

            mechMoveController.setTargetLook(CurrLookRotation);

            mechMoveController.setCurrMoveInput(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));

            mechMoveController.setIsFiring(Input.GetButton("Fire1"));

            if(adsImage) {
                adsImage.SetActive(Input.GetButton("Fire1"));
            }

            if(Input.GetButtonDown("Fire3")) {
                mechMoveController.setAttemptDash();
            }

            if(Input.GetKeyDown("q")) {
                mechMoveController.setIsVenting();
            }

            if(Input.GetKeyDown("e")) {
                mechMoveController.setIsShielding();
            }
        }
        cameraTransform.localRotation = Quaternion.Euler(freelookRotation.y + camshakeRotOffset.x, freelookRotation.x + camshakeRotOffset.y, 0f);
        camRootOuter.localRotation = Quaternion.Euler(0f,CurrLookRotation.x,0f);
        transform.localRotation = Quaternion.Euler(-1f * CurrLookRotation.y, 0f, 0f);
        pilotPovCamera.fieldOfView = Mathf.SmoothDamp(pilotPovCamera.fieldOfView, targetFov, ref camVel, 0.1f);
        // print("FLR: (" + freelookRotation.x + ", " + freelookRotation.y + "),   CT.LR: (" + cameraTransform.localRotation.x + ", " + cameraTransform.localRotation.y + ", " + cameraTransform.localRotation.z + ")");

        if(Input.GetKeyDown("r")) {
            Application.LoadLevel(Application.loadedLevel);
        }

        if(Input.GetKeyDown("g")) {
            myLauncherController.OnTryFire();
        }

        if(Input.GetKeyDown("f")) {
            myRocketLauncherController.OnTryFire();
        }

        if(isShaking) {
            simulateCurrentShake();
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

    public void addShake(float nintensity, float duration, Vector3 direction = new Vector3(), float newshaketime = 0.002f) {
        isShaking = true;
        camshakeRotOffset = new Vector3(0f,0f,0f);
        // set values for intensity and duration
        intensity = nintensity;
        currShakePercent = 0f;
        currShakeElapseFactor = 1f / duration;
        singleShakeTime = newshaketime;
        camshakeRotGoal = (direction.normalized + new Vector3(Random.Range(-.25f,.25f),Random.Range(-.25f,.25f),0f)).normalized * (intensity * screenshakeAnimationCurve.Evaluate(currShakePercent));
    }

    public void simulateCurrentShake() {
        if(camshakeRotOffset == camshakeRotGoal) {
            camshakeRotGoal = ((camshakeRotGoal * -1f).normalized + new Vector3(Random.Range(-.25f,.25f),Random.Range(-.25f,.25f),0f)).normalized * (intensity * screenshakeAnimationCurve.Evaluate(currShakePercent));
            camSpeedByFrequency = screenshakeAnimationCurve.Evaluate(currShakePercent)*300f;
        }
        camshakeRotOffset = Vector3.SmoothDamp(camshakeRotOffset,camshakeRotGoal,ref currShakeV, singleShakeTime);
        currShakePercent += Time.deltaTime*currShakeElapseFactor;
        if(currShakePercent >= 1f) {
            isShaking = false;
            camshakeRotOffset = new Vector3(0f,0f,0f);
            camshakeRotGoal = new Vector3(0f,0f,0f);
        }
    }

    // must add the camshakeRotOffset to freelook rotation in update()
}
