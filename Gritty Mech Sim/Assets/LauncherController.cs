using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherController : MonoBehaviour
{
    private int capacity = 3;
    private float cooldown = 1f; // in seconds
    private float launchVelocity = 5f;
    [SerializeField] private GameObject launchedItemPrefab;
    private Transform launcher;

    private bool tryFire = false;
    private int currCapacity = 3;
    private float currCooldown = 1f;

    // Start is called before the first frame update
    void Start()
    {
        launcher = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(tryFire && currCapacity > 0 && cooldown <= 0f) {
            currCapacity -= 1;
            currCooldown = cooldown;
            tryFire = false;
        }
        currCooldown = Mathf.Clamp(currCooldown - Time.deltaTime, 0f, cooldown);
    }

    public void onTryFire() {
        tryFire = true;
    }

    private void fireProjectile() {
        GameObject projectile = Instantiate(launchedItemPrefab,launcher.position,launcher.rotation);
        if(projectile.GetComponent<Rigidbody>()) {
            projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * launchVelocity;
        }
        // play fire sound, effects, etc..
    }

    //
}
