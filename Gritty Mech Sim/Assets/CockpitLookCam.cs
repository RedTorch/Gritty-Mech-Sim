using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitLookCam : MonoBehaviour
{
    private Vector2 CurrLookRotation = new Vector2(0f,0f);
    private float LookSpeed = 3f;
    private bool isPiloting = true;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera pilotPovCamera;
    private float pilotingFov = 60f;
    private float freelookFov = 75f;
    private float zoomedFov = 40f;
    private float targetFov = 60f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CurrLookRotation.x += Input.GetAxis("Mouse X") * LookSpeed;
        CurrLookRotation.y = Mathf.Clamp(CurrLookRotation.y + (Input.GetAxis("Mouse Y") * LookSpeed),-80f,80f);
        transform.localRotation = Quaternion.Euler(0f,CurrLookRotation.x,0f);
        cameraTransform.localRotation = Quaternion.Euler(-1f * CurrLookRotation.y, 0f, 0f);
        if(Input.GetKey(KeyCode.Tab)) {
            isPiloting = false;
            // pilotPovCamera.fieldOfView = 75f;
        } else {
            isPiloting = true;
            pilotPovCamera.fieldOfView = 60f;
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

    public Vector2 getLookRotation() {
        return CurrLookRotation;
    }

    public bool getIsPiloting() {
        return isPiloting;
    }
}
