using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherController : MonoBehaviour
{
    [SerializeField] private int capacity = 3;
    [SerializeField] private float cooldown = 1f; // in seconds
    [SerializeField] private float reloadCooldown = 3f;
    private float launchVelocity = 40f;
    [SerializeField] private GameObject launchedItemPrefab;
    [SerializeField] private string displayName = "GRN"; // this should be 3 characters or less to fit on the screen!
    private Transform launcher;

    private bool tryFire = false;
    private int currCapacity = 3;
    private float currCooldown = 0f;
    private float currReload = 0f;

    private float currentBarMax = 0f;

    // Start is called before the first frame update
    void Start()
    {
        launcher = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(tryFire && currCapacity > 0 && currCooldown <= 0f && currReload >= reloadCooldown) {
            fireProjectile();
            currCapacity -= 1;
            if(currCapacity <= 0) {
                currCapacity = capacity;
                currReload = 0f;
            } else {
                currCooldown = cooldown;
            }
            tryFire = false;
        }
        currCooldown = Mathf.Clamp(currCooldown - Time.deltaTime, 0f, cooldown);
        currReload = Mathf.Clamp(currReload + Time.deltaTime, 0f, reloadCooldown);
    }

    public void OnTryFire() {
        tryFire = true;
    }

    private void fireProjectile() {
        GameObject projectile = Instantiate(launchedItemPrefab,launcher.position,launcher.rotation);
        if(projectile.GetComponent<Rigidbody>()) {
            projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * launchVelocity;
        }
        // play fire sound, effects, etc..
    }

    public string getText() {
        return $"{displayName}-{currCapacity}";
    }

    public float getCurrPercent() {
        return currReload/reloadCooldown;
    }
}
