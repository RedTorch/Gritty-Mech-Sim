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

    private float tiltFactor = 5f;
    private float currTilt = 0f;
    private float tiltVelocity = 0f;

    private float heat = 0f; // DISPLAY
    private float maxHeat = 10f;
    private float dashHeat = 1.5f;
    private float shieldHeat = 1.5f;

    private bool isShielding = false; // DISPLAY ALL THESE
    private bool isHeatVenting = false;
    private bool canDash = false;

    private float currShield = 0f; // DISPLAY
    private float maxShield = 100f;
    private float shieldRecovery = 20f;

    private float health = 100f; // DISPLAY
    private float maxHealth = 100f;
    private bool isAlive = true;

    private float attackRange = 100f;
    private float currFireInterval = 0f;
    private float fireInterval = 0.25f;
    [SerializeField] private GameObject bulletPrefab;
    private float weaponOverheat = 0f;
    private float weaponOverheatMax = 10f;

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
        if(!isAlive) {
            return;
        }
        if(pilotLookCam.getIsPiloting()) {
            Vector2 targetLook = pilotLookCam.getLookRotation();
            // CurrLookRotation.x = Mathf.MoveTowardsAngle(CurrLookRotation.x, targetLook.x, lookSpeed*Time.deltaTime);
            // CurrLookRotation.y = Mathf.Clamp(Mathf.MoveTowardsAngle(CurrLookRotation.y, targetLook.y, lookSpeed*Time.deltaTime),-30f,30f);
            CurrLookRotation.x = Mathf.Lerp(CurrLookRotation.x, targetLook.x, 10f*Time.deltaTime);
            CurrLookRotation.y = Mathf.Clamp(Mathf.Lerp(CurrLookRotation.y, targetLook.y, 10f*Time.deltaTime),-30f,30f);
            if(Input.GetButton("Fire1")) {
                tryFireGun();
            }
        }
        currTilt = Mathf.SmoothDamp(currTilt, (CurrVelocity.x/MoveSpeed)*(-1f)*tiltFactor, ref tiltVelocity, 0.2f);
        transform.localRotation = Quaternion.Euler(0f,CurrLookRotation.x,0f);
        lookRoot.transform.localRotation = Quaternion.Euler(-1f * CurrLookRotation.y, 0f, currTilt);
        cockpitRotationRoot.localRotation = Quaternion.Euler(-1f * CurrLookRotation.y,CurrLookRotation.x,0f);

        // Disable systems based on max heat
        canDash = heat < (maxHeat - dashHeat);

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
            if(Input.GetButtonDown("Fire3") && canDash) {
                setHeat(heat + dashHeat);
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

        if(Input.GetKeyDown("q")) {
            isHeatVenting = !isHeatVenting;
        }

        if(Input.GetKeyDown("e")) {
            isShielding = !isShielding;
        }
        if(heat > (maxHeat - 0.5f) || isHeatVenting) {
            isShielding = false;
        }

        if(isShielding) {
            setHeat(heat + (shieldHeat*Time.deltaTime));
            currShield += shieldRecovery * Time.deltaTime;
        }
        else {
            currShield -= shieldRecovery * 0.1f * Time.deltaTime;
        }
        currShield = Mathf.Clamp(currShield,0f,maxShield);

        // heat calculations
        if(heat > 0f) {
            float recoveryMultiplier = 1f;
            if(CurrVelocity.magnitude == 0f) {
                recoveryMultiplier *= 1.5f;
            }
            if(isHeatVenting) {
                recoveryMultiplier *= 2f;
            }
            setHeat(heat -= (recoveryMultiplier * Time.deltaTime));
        }

        weaponOverheat = Mathf.Clamp(weaponOverheat-Time.deltaTime,0f,weaponOverheatMax);
    }

    public float getLookY() {
        return CurrLookRotation.y;
    }

    public void setHeat(float newHeat) {
        heat = Mathf.Clamp(newHeat,0f,maxHeat);
    }

    public void receiveDamage(float dmg) {
        if(isShielding) {
            if(currShield > dmg) {
                currShield -= dmg;
                return;
            }
            else {
                dmg -= currShield;
                currShield = 0f;
            }
        }
        else if(isHeatVenting) {
            dmg *= 3f;
        }
        health -= dmg;
        if(health <= 0f) {
            death();
        }
    }

    public void death() {
        isAlive = false;
    }

    public float getCompassPercent() {
        if(CurrLookRotation.x > 360f) {
            return (CurrLookRotation.x % 360f)/360f;
        }
        else if(CurrLookRotation.x < 360f) {
            return ((CurrLookRotation.x + 1080f) % 360f)/360f;
        }
        return CurrLookRotation.x/360f;
    }

    public float getHeatPercent() {
        return Mathf.Floor(100f*heat/maxHeat);
    }

    public float getHealthPercent() {
        return Mathf.Floor(100f*health/maxHealth);
    }

    public float getShieldPercent() {
        return Mathf.Floor(100f*currShield/maxShield);
    }

    public bool getIsHeatVenting() {
        return isHeatVenting;
    }

    public bool getIsShielding() {
        return isShielding;
    }

    public float getWeaponPercent() {
        return Mathf.Floor(100f*weaponOverheat/weaponOverheatMax);
    }

    void tryFireGun() {
        if(weaponOverheat >= weaponOverheatMax) {
            return;
        }
        currFireInterval -= Time.deltaTime;
        if(currFireInterval<=0f) {
            Instantiate(bulletPrefab, transform.position, transform.rotation);
            currFireInterval += fireInterval;
            weaponOverheat += 0.5f;
        }
    }
}